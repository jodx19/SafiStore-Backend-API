using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SafiStore.Api.Application.DTOs;
using SafiStore.Api.Common.Extensions;
using SafiStore.Api.Infrastructure.Services;
using System.Threading.Tasks;

namespace SafiStore.Api.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    [Authorize]  // All cart operations require authentication
    public class CartController : ControllerBase
    {
        private readonly ICartService _cartService;
        public CartController(ICartService cartService) => _cartService = cartService;

        /// <summary>Get the current user's cart (items + total).</summary>
        [HttpGet]
        public async Task<ActionResult<ApiResponse<object>>> GetCart()
        {
            var userId = User.GetUserIdOrThrow();
            var items  = await _cartService.GetUserCartAsync(userId);
            var total  = await _cartService.GetCartTotalAsync(userId);

            var payload = new { items, total };
            return Ok(ApiResponse<object>.Ok(payload, "Cart retrieved successfully"));
        }

        /// <summary>Add an item to the cart.</summary>
        [HttpPost("items")]
        public async Task<ActionResult<ApiResponse<CartItemDto>>> AddToCart([FromBody] AddToCartDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ApiResponse<CartItemDto>.Error("VALIDATION_ERROR", "Invalid input", ModelState));

            var userId = User.GetUserIdOrThrow();
            var result = await _cartService.AddToCartAsync(userId, dto.ProductId, dto.Quantity);

            if (!result.Success)
                return BadRequest(ApiResponse<CartItemDto>.Error(result.ErrorCode ?? "CART_ERROR", result.Message ?? "Unable to add item to cart"));

            return Ok(ApiResponse<CartItemDto>.Ok(result.Data!, result.Message ?? "Item added to cart"));
        }

        /// <summary>Update the quantity of a cart item.</summary>
        [HttpPut("items/{cartItemId:int}")]
        public async Task<ActionResult<ApiResponse<CartItemDto>>> UpdateItem(int cartItemId, [FromBody] UpdateCartItemDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ApiResponse<CartItemDto>.Error("VALIDATION_ERROR", "Invalid input", ModelState));

            var userId = User.GetUserIdOrThrow();
            try
            {
                var item = await _cartService.UpdateCartItemAsync(userId, cartItemId, dto.Quantity);
                return Ok(ApiResponse<CartItemDto>.Ok(item, "Cart item updated"));
            }
            catch (ArgumentException ex)          { return BadRequest(ApiResponse<CartItemDto>.Error("INVALID", ex.Message)); }
            catch (InvalidOperationException ex)  { return BadRequest(ApiResponse<CartItemDto>.Error("BUSINESS_ERROR", ex.Message)); }
        }

        /// <summary>Remove a single item from the cart.</summary>
        [HttpDelete("items/{cartItemId:int}")]
        public async Task<ActionResult<ApiResponse<object>>> RemoveItem(int cartItemId)
        {
            var userId = User.GetUserIdOrThrow();
            try
            {
                await _cartService.RemoveFromCartAsync(userId, cartItemId);
                return Ok(ApiResponse<object>.Ok(null, "Item removed"));
            }
            catch (ArgumentException ex) { return BadRequest(ApiResponse<object>.Error("INVALID", ex.Message)); }
        }

        /// <summary>Clear all items from the current user's cart.</summary>
        [HttpDelete]
        public async Task<ActionResult<ApiResponse<object>>> ClearCart()
        {
            var userId = User.GetUserIdOrThrow();
            await _cartService.ClearCartAsync(userId);
            return Ok(ApiResponse<object>.Ok(null, "Cart cleared"));
        }

        // NOTE: GET /cart/total was removed (DESIGN-3).
        // The total is already included in the GET /cart response payload.
    }
}
