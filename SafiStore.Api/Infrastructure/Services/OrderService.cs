using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SafiStore.Api.Application.DTOs;
using SafiStore.Api.Data;
using SafiStore.Api.Common.Extensions;
using SafiStore.Api.Models.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SafiStore.Api.Infrastructure.Services
{
    public class OrderService : IOrderService
    {
        private readonly AppDbContext _context;
        private readonly ILogger<OrderService> _logger;
        private readonly INotificationService _notificationService;

        public OrderService(AppDbContext context, ILogger<OrderService> logger, INotificationService notificationService)
        {
            _context = context;
            _logger = logger;
            _notificationService = notificationService;
        }

        public async Task<SafiStore.Api.Application.DTOs.ServiceResult<int>> CreateOrderAsync(CreateOrderDto dto)
        {
            // حل مشكلة التعارض بين Transaction و Execution Strategy
            var strategy = _context.Database.CreateExecutionStrategy();

            return await strategy.ExecuteAsync(async () =>
            {
                await using var transaction = await _context.Database.BeginTransactionAsync();

                try
                {
                    // 1. جلب الكارت مع المنتجات
                    var cart = await _context.Carts
                        .Include(c => c.Items)
                            .ThenInclude(ci => ci.Product)
                        .FirstOrDefaultAsync(c => c.UserId == dto.UserId);

                    if (cart == null || cart.Items == null || !cart.Items.Any())
                        throw new InvalidOperationException("Cart is empty");

                    // 2. تحقق المخزون
                    foreach (var ci in cart.Items)
                    {
                        if (ci.Product == null)
                            throw new InvalidOperationException($"Product (Id={ci.ProductId}) not found");

                        if (ci.Product.Stock < ci.Quantity)
                            throw new InvalidOperationException($"Insufficient stock for product '{ci.Product.Title ?? ci.ProductId.ToString()}'");
                    }

                    // 3. حساب الإجمالي بناءً على السعر وقت الإضافة
                    decimal total = cart.Items.Sum(ci => ci.Quantity * ci.PriceAtAddition);

                    // 4. إنشاء سجل الطلب الرئيسي
                    var order = new Order
                    {
                        UserId = dto.UserId,
                        ShippingAddress = dto.ShippingAddress,
                        City = dto.City,
                        Country = dto.Country,
                        PostalCode = dto.PostalCode,
                        PaymentMethod = dto.PaymentMethod,
                        Status = "Pending",
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow,
                        TotalAmount = total
                    };

                    _context.Orders.Add(order);
                    await _context.SaveChangesAsync();

                    // 5. إنشاء تفاصيل الطلب وتحديث المخزون
                    foreach (var ci in cart.Items)
                    {
                        var orderItem = new OrderItem
                        {
                            OrderId = order.Id,
                            ProductId = ci.ProductId,
                            Quantity = ci.Quantity,
                            UnitPrice = ci.PriceAtAddition
                        };
                        _context.OrderItems.Add(orderItem);

                        // خصم الكمية من جدول المنتجات
                        if (ci.Product != null)
                        {
                            ci.Product.Stock -= ci.Quantity;
                            _context.Products.Update(ci.Product);
                        }
                    }

                    // 6. تفريغ السلة بعد نجاح الطلب
                    _context.CartItems.RemoveRange(cart.Items);

                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();

                    // 7. إشعار الأدمن بالطلب الجديد
                    await NotifyAdminsAsync("new_order", $"New order #{order.Id} placed - ${total:F2}", order.Id);

                    _logger.LogInformation("Order {OrderId} created successfully for User {UserId}", order.Id, dto.UserId);

                    return Application.DTOs.ServiceResult<int>.SuccessResult(order.Id, "Order created successfully");
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    _logger.LogError(ex, "Failed to create order for User {UserId}", dto.UserId);
                    // إرجاع تفاصيل الخطأ للمساعدة في التوثيق
                    return Application.DTOs.ServiceResult<int>.Fail("ORDER_CREATION_FAILED", $"Error: {ex.Message}");
                }
            });
        }

        public async Task<OrderDto?> GetOrderByIdAsync(int id)
        {
            var o = await _context.Orders
                .AsNoTracking()
                .Include(x => x.Items)
                    .ThenInclude(oi => oi.Product)
                .Where(x => x.Id == id)
                .FirstOrDefaultAsync();

            if (o == null) return null;

            return new OrderDto
            {
                Id = o.Id,
                UserId = o.UserId,
                ShippingAddress = o.ShippingAddress,
                City = o.City,
                Country = o.Country,
                PostalCode = o.PostalCode,
                Status = o.Status,
                CreatedAt = o.CreatedAt,
                TotalAmount = o.TotalAmount,
                Items = o.Items?.Select(i => new OrderItemDto
                {
                    ProductId = i.ProductId,
                    Quantity = i.Quantity
                }).ToList()
            };
        }

        public async Task<List<OrderDto>> GetOrdersForUserAsync(int userId)
        {
            return await _context.Orders
                .AsNoTracking()
                .Where(o => o.UserId == userId)
                .OrderByDescending(o => o.CreatedAt)
                .Select(o => new OrderDto
                {
                    Id = o.Id,
                    UserId = o.UserId,
                    ShippingAddress = o.ShippingAddress,
                    City = o.City,
                    Country = o.Country,
                    PostalCode = o.PostalCode,
                    Status = o.Status,
                    CreatedAt = o.CreatedAt,
                    TotalAmount = o.TotalAmount
                })
                .ToListAsync();
        }

        public async Task<(List<OrderDto> Orders, int Total)> GetAllOrdersAsync(int page = 1, int pageSize = 20)
        {
            var query = _context.Orders.AsNoTracking();

            int total = await query.CountAsync();

            var orders = await query
                .OrderByDescending(o => o.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(o => new OrderDto
                {
                    Id = o.Id,
                    UserId = o.UserId,
                    ShippingAddress = o.ShippingAddress,
                    City = o.City,
                    Country = o.Country,
                    PostalCode = o.PostalCode,
                    Status = o.Status,
                    CreatedAt = o.CreatedAt,
                    TotalAmount = o.TotalAmount
                })
                .ToListAsync();

            return (orders, total);
        }

        public async Task<OrderDto> UpdateOrderStatusAsync(int orderId, string status)
        {
            var order = await _context.Orders
                .Include(o => o.Items)
                    .ThenInclude(oi => oi.Product)
                .FirstOrDefaultAsync(o => o.Id == orderId);
            if (order == null) throw new ArgumentException("Order not found");

            // Restore product stock when cancelling an order
            if (status == "Cancelled" && order.Status != "Cancelled")
            {
                foreach (var item in order.Items)
                {
                    if (item.Product != null)
                    {
                        item.Product.Stock += item.Quantity;
                        _context.Products.Update(item.Product);
                        _logger.LogInformation("Restored {Quantity} units of Product {ProductId} from cancelled Order {OrderId}",
                            item.Quantity, item.ProductId, orderId);
                    }
                }
            }

            order.Status = status;
            order.UpdatedAt = DateTime.UtcNow;
            if (status == "Shipped") order.ShippedAt = DateTime.UtcNow;
            if (status == "Delivered") order.DeliveredAt = DateTime.UtcNow;
            _context.Orders.Update(order);
            await _context.SaveChangesAsync();

            // إشعار المستخدم بتغيير حالة الطلب
            await _notificationService.CreateNotificationAsync(
                order.UserId,
                "order_status",
                $"Order #{order.Id} status updated to {status}",
                order.Id);

            return new OrderDto
            {
                Id = order.Id,
                UserId = order.UserId,
                ShippingAddress = order.ShippingAddress,
                City = order.City,
                Country = order.Country,
                PostalCode = order.PostalCode,
                Status = order.Status,
                CreatedAt = order.CreatedAt,
                TotalAmount = order.TotalAmount
            };
        }
        private async Task NotifyAdminsAsync(string type, string message, int? orderId = null)
        {
            try
            {
                var adminUsers = await _context.Users
                    .Where(u => u.Role == "Admin" && u.IsActive)
                    .ToListAsync();

                foreach (var admin in adminUsers)
                {
                    await _notificationService.CreateNotificationAsync(admin.Id, type, message, orderId);
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to notify admins about order event");
            }
        }
    }
}
