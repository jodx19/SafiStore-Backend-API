using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SafiStore.Api.Application.DTOs;
using SafiStore.Api.Data;
using SafiStore.Api.Common.Extensions;
using SafiStore.Api.Models.Domain;

namespace SafiStore.Api.Infrastructure.Services
{
    public class CartService : ICartService
    {
        private readonly ApplicationDbContext _context;

        public CartService(ApplicationDbContext context)
        {
            _context = context;
        }

        private static DateTimeOffset MapAddedAt(CartItem ci)
        {
            return new DateTimeOffset(ci.AddedAt);
        }

        public async Task<List<CartItemDto>> GetUserCartAsync(int userId)
        {
            var cart = await _context.Carts
                .Include(c => c.Items)
                .ThenInclude(ci => ci.Product)
                .FirstOrDefaultAsync(c => c.UserId == userId);

            if (cart == null) return new List<CartItemDto>();

            return cart.Items.Select(ci => new CartItemDto
            {
                Id = ci.Id,
                ProductId = ci.ProductId,
                ProductTitle = ci.Product?.Title ?? string.Empty,
                ProductImage = ci.Product?.ImageUrl ?? string.Empty,
                Quantity = ci.Quantity,
                PriceAtAddition = ci.PriceAtAddition,
                TotalPrice = ci.Quantity * ci.PriceAtAddition,
                AddedAt = MapAddedAt(ci)
            }).ToList();
        }

        public async Task<SafiStore.Api.Application.DTOs.ServiceResult<CartItemDto>> AddToCartAsync(int userId, int productId, int quantity)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null) throw new ArgumentException("User not found");

            var product = await _context.Products.FindAsync(productId);
            if (product == null) throw new ArgumentException("Product not found");
            if (product.Stock < quantity) throw new InvalidOperationException("Insufficient stock");

            // الحصول على الـ Cart الخاص بالمستخدم أو إنشاء واحد جديد
            var cart = await _context.Carts
                .Include(c => c.Items)
                .FirstOrDefaultAsync(c => c.UserId == userId);

            if (cart == null)
            {
                cart = new Cart
                {
                    UserId = userId,
                    Status = "Active",
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };
                _context.Carts.Add(cart);
                try
                {
                    await _context.SaveChangesAsync();
                }
                catch (Microsoft.EntityFrameworkCore.DbUpdateException dbEx) when (dbEx.IsUniqueConstraintViolation())
                {
                    // Race condition: another request may have created a cart for this user between
                    // our pre-check and SaveChanges. Recover by reloading the persisted cart.
                    await _context.Entry(cart).ReloadAsync();
                    cart = await _context.Carts.Include(c => c.Items).FirstOrDefaultAsync(c => c.UserId == userId) ?? cart;
                }
                catch (Microsoft.EntityFrameworkCore.DbUpdateException)
                {
                    return Application.DTOs.ServiceResult<CartItemDto>.Fail("CART_CREATE_FAILED", "Unable to create cart");
                }
            }

            // تحقق إذا كان المنتج موجود بالفعل في السلة
            var existing = cart.Items.FirstOrDefault(ci => ci.ProductId == productId);

            CartItem cartItem;
            if (existing != null)
            {
                existing.Quantity += quantity;
                cartItem = existing;
            }
            else
            {
                cartItem = new CartItem
                {
                    CartId = cart.Id,
                    ProductId = productId,
                    Quantity = quantity,
                    PriceAtAddition = product.Price,
                    AddedAt = DateTime.UtcNow
                };
                _context.CartItems.Add(cartItem);
            }

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (Microsoft.EntityFrameworkCore.DbUpdateException dbEx) when (dbEx.IsUniqueConstraintViolation())
            {
                // Race condition: concurrent add of the same cart item may cause unique index violation.
                var existingNow = await _context.CartItems.FirstOrDefaultAsync(ci => ci.CartId == cart.Id && ci.ProductId == productId);
                if (existingNow != null)
                {
                    existingNow.Quantity += quantity;
                    await _context.SaveChangesAsync();
                    cartItem = existingNow;
                }
                else
                {
                    return Application.DTOs.ServiceResult<CartItemDto>.Fail("CART_UPDATE_FAILED", "Unable to update cart item");
                }
            }
            catch (Microsoft.EntityFrameworkCore.DbUpdateException)
            {
                return Application.DTOs.ServiceResult<CartItemDto>.Fail("CART_UPDATE_FAILED", "Unable to update cart item");
            }
            await _context.Entry(cartItem).Reference(ci => ci.Product).LoadAsync();

            return Application.DTOs.ServiceResult<CartItemDto>.SuccessResult(new CartItemDto
            {
                Id = cartItem.Id,
                ProductId = cartItem.ProductId,
                ProductTitle = cartItem.Product?.Title ?? string.Empty,
                ProductImage = cartItem.Product?.ImageUrl ?? string.Empty,
                Quantity = cartItem.Quantity,
                PriceAtAddition = cartItem.PriceAtAddition,
                TotalPrice = cartItem.Quantity * cartItem.PriceAtAddition,
                AddedAt = MapAddedAt(cartItem)
            }, "Item added to cart");
        }

        public async Task<CartItemDto> UpdateCartItemAsync(int userId, int cartItemId, int quantity)
        {
            var cart = await _context.Carts
                .Include(c => c.Items)
                .ThenInclude(ci => ci.Product)
                .FirstOrDefaultAsync(c => c.UserId == userId);

            if (cart == null) throw new ArgumentException("Cart not found");

            var item = cart.Items.FirstOrDefault(ci => ci.Id == cartItemId);
            if (item == null) throw new ArgumentException("Cart item not found");
            if (quantity <= 0) throw new ArgumentException("Quantity must be positive");
            if (item.Product != null && item.Product.Stock < quantity) throw new InvalidOperationException("Insufficient stock");

            item.Quantity = quantity;
            item.Cart.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            return new CartItemDto
            {
                Id = item.Id,
                ProductId = item.ProductId,
                ProductTitle = item.Product?.Title ?? string.Empty,
                ProductImage = item.Product?.ImageUrl ?? string.Empty,
                Quantity = item.Quantity,
                PriceAtAddition = item.PriceAtAddition,
                TotalPrice = item.Quantity * item.PriceAtAddition,
                AddedAt = MapAddedAt(item)
            };
        }

        public async Task RemoveFromCartAsync(int userId, int cartItemId)
        {
            var cart = await _context.Carts
                .Include(c => c.Items)
                .FirstOrDefaultAsync(c => c.UserId == userId);

            if (cart == null) throw new ArgumentException("Cart not found");

            var item = cart.Items.FirstOrDefault(ci => ci.Id == cartItemId);
            if (item == null) throw new ArgumentException("Cart item not found");

            _context.CartItems.Remove(item);
            cart.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
        }

        public async Task ClearCartAsync(int userId)
        {
            var cart = await _context.Carts
                .Include(c => c.Items)
                .FirstOrDefaultAsync(c => c.UserId == userId);

            if (cart == null) return;

            _context.CartItems.RemoveRange(cart.Items);
            cart.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
        }

        public async Task<decimal> GetCartTotalAsync(int userId)
        {
            var cart = await _context.Carts
                .AsNoTracking()
                .Include(c => c.Items)
                .FirstOrDefaultAsync(c => c.UserId == userId);

            if (cart == null) return 0m;

            return cart.Items.Sum(ci => ci.Quantity * ci.PriceAtAddition);
        }
    }
}
