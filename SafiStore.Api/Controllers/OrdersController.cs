using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SafiStore.Api.Application.DTOs;
using SafiStore.Api.Common.Extensions;
using SafiStore.Api.Infrastructure.Services;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SafiStore.Api.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    [Authorize]
    public class OrdersController : ControllerBase
    {
        private readonly IOrderService _orderService;
        public OrdersController(IOrderService orderService) => _orderService = orderService;

        /// <summary>Create a new order from the current user's cart.</summary>
        [HttpPost]
        public async Task<ActionResult<ApiResponse<int>>> CreateOrder([FromBody] CreateOrderDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ApiResponse<int>.Error("VALIDATION_ERROR", "Invalid order data", ModelState));

            var userId  = User.GetUserIdOrThrow();
            dto.UserId  = userId;  // enforce server-side ownership — never trust client-supplied userId

            var result = await _orderService.CreateOrderAsync(dto);

            if (!result.Success)
                return BadRequest(ApiResponse<int>.Error(result.ErrorCode ?? "ORDER_CREATION_FAILED", result.Message ?? "Unable to create order"));

            return Ok(ApiResponse<int>.Ok(result.Data, result.Message ?? "Order created successfully"));
        }

        /// <summary>Get a specific order by ID. Only the owner or an Admin may access it.</summary>
        [HttpGet("{id:int}")]
        public async Task<ActionResult<ApiResponse<OrderDto>>> GetOrderById(int id)
        {
            var order = await _orderService.GetOrderByIdAsync(id);
            if (order == null)
                return NotFound(ApiResponse<OrderDto>.Error("NOT_FOUND", "Order not found"));

            var userId  = User.GetUserIdOrThrow();
            var isAdmin = User.IsInRole("Admin");
            if (order.UserId != userId && !isAdmin)
                return Forbid();

            return Ok(ApiResponse<OrderDto>.Ok(order, "Order retrieved"));
        }

        /// <summary>Get all orders for the currently authenticated user.</summary>
        [HttpGet]
        public async Task<ActionResult<ApiResponse<List<OrderDto>>>> GetMyOrders()
        {
            // AUTH-1 fix: always scope to the authenticated user's ID from claims.
            // Admin can see all orders via GET /api/v1/admin/orders instead.
            var userId = User.GetUserIdOrThrow();
            var orders = await _orderService.GetOrdersForUserAsync(userId);
            return Ok(ApiResponse<List<OrderDto>>.Ok(orders, "Orders retrieved"));
        }
    }
}
