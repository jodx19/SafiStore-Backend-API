using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using SafiStore.Api.Application.DTOs;
using SafiStore.Api.Infrastructure.Services;
using SafiStore.Api.Models.Domain;
using SafiStore.Api.Data;

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
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly AppDbContext _context;

        public AdminController(
            IProductService productService,
            IOrderService orderService,
            IUserService userService,
            UserManager<ApplicationUser> userManager,
            AppDbContext context)
        {
            _productService = productService;
            _orderService = orderService;
            _userService = userService;
            _userManager = userManager;
            _context = context;
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

        /// <summary>Create a new admin user (Admin only).</summary>
        [HttpPost("users")]
        public async Task<ActionResult<ApiResponse<object>>> CreateAdmin([FromBody] CreateAdminRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ApiResponse<object>.Error("VALIDATION_ERROR", "Invalid data", ModelState));

            var existingUser = await _userManager.FindByEmailAsync(request.Email);
            if (existingUser != null)
                return Conflict(ApiResponse<object>.Error("DUPLICATE_EMAIL", "Email already registered"));

            var user = new ApplicationUser
            {
                UserName = request.Email,
                Email = request.Email,
                FirstName = request.FirstName,
                LastName = request.LastName,
                CreatedAt = System.DateTime.UtcNow,
                EmailConfirmed = true
            };

            var result = await _userManager.CreateAsync(user, request.Password);
            if (!result.Succeeded)
                return BadRequest(ApiResponse<object>.Error("CREATION_FAILED",
                    "Failed to create user: " + string.Join(", ", result.Errors.Select(e => e.Description))));

            await _userManager.AddToRoleAsync(user, "Admin");
            user.Role = "Admin";
            await _userManager.UpdateAsync(user);

            return Ok(ApiResponse<object>.Ok(new { user.Id, user.Email, user.FirstName, user.LastName }, "Admin created successfully"));
        }
    }

    public class CreateAdminRequest
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }
}
