using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System.Collections.Generic;
using SafiStore.Api.Application.DTOs;
using SafiStore.Api.Infrastructure.Services;

namespace SafiStore.Api.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class AdminController : ControllerBase
    {
        private readonly IProductService _productService;
        private readonly IOrderService _orderService;
        private readonly IUserService _userService;

        public AdminController(
            IProductService productService,
            IOrderService orderService,
            IUserService userService)
        {
            _productService = productService;
            _orderService = orderService;
            _userService = userService;
        }

        /// <summary>Get all orders (Admin only).</summary>
        [HttpGet("orders")]
        public async Task<ActionResult<ApiResponse<object>>> GetAllOrders(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20)
        {
            if (page < 1) page = 1;
            if (pageSize < 1 || pageSize > 100) pageSize = 20;

            var (orders, total) = await _orderService.GetAllOrdersAsync(page, pageSize);
            
            var payload = new 
            {
                orders,
                pagination = new 
                {
                    page,
                    pageSize,
                    total,
                    totalPages = (int)System.Math.Ceiling((double)total / pageSize)
                }
            };
            
            return Ok(ApiResponse<object>.Ok(payload, "Orders retrieved successfully"));
        }

        /// <summary>Get all users (Admin only).</summary>
        [HttpGet("users")]
        public async Task<ActionResult<ApiResponse<object>>> GetAllUsers(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20)
        {
            if (page < 1) page = 1;
            if (pageSize < 1 || pageSize > 100) pageSize = 20;

            var (users, total) = await _userService.GetAllUsersAsync(page, pageSize);

            var payload = new 
            {
                users,
                pagination = new 
                {
                    page,
                    pageSize,
                    total,
                    totalPages = (int)System.Math.Ceiling((double)total / pageSize)
                }
            };

            return Ok(ApiResponse<object>.Ok(payload, "Users retrieved successfully"));
        }

        /// <summary>Delete a user by ID (Admin only).</summary>
        [HttpDelete("users/{id:int}")]
        public async Task<ActionResult<ApiResponse<object>>> DeleteUser(int id)
        {
            try
            {
                await _userService.DeleteUserAsync(id);
                return Ok(ApiResponse<object>.Ok(null, "User deleted successfully"));
            }
            catch (System.ArgumentException)
            {
                return NotFound(ApiResponse<object>.Error("NOT_FOUND", "User not found"));
            }
        }

        /// <summary>Update the status of an order (e.g., Pending → Shipped).</summary>
        [HttpPut("orders/{id:int}/status")]
        public async Task<ActionResult<ApiResponse<OrderDto>>> UpdateOrderStatus(int id, [FromBody] UpdateOrderStatusDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ApiResponse<OrderDto>.Error("VALIDATION_ERROR", "Invalid status data", ModelState));

            try
            {
                var updatedOrder = await _orderService.UpdateOrderStatusAsync(id, dto.Status);
                return Ok(ApiResponse<OrderDto>.Ok(updatedOrder, "Order status updated successfully"));
            }
            catch (System.ArgumentException ex)
            {
                return NotFound(ApiResponse<OrderDto>.Error("NOT_FOUND", ex.Message));
            }
            catch (System.InvalidOperationException ex)
            {
                return BadRequest(ApiResponse<OrderDto>.Error("BUSINESS_ERROR", ex.Message));
            }
        }

        /// <summary>Cancel an order (Admin only).</summary>
        [HttpPost("orders/{id:int}/cancel")]
        public async Task<ActionResult<ApiResponse<OrderDto>>> CancelOrder(int id)
        {
            try
            {
                var cancelledOrder = await _orderService.UpdateOrderStatusAsync(id, "Cancelled");
                
                // TODO: Restore product stock if needed
                // This would require adding stock restoration logic to OrderService
                
                return Ok(ApiResponse<OrderDto>.Ok(cancelledOrder, "Order cancelled successfully"));
            }
            catch (System.ArgumentException ex)
            {
                return NotFound(ApiResponse<OrderDto>.Error("NOT_FOUND", ex.Message));
            }
            catch (System.InvalidOperationException ex)
            {
                return BadRequest(ApiResponse<OrderDto>.Error("BUSINESS_ERROR", ex.Message));
            }
        }
    }
}
