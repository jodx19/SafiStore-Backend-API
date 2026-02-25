
// Services/ICartService.cs
using System.Collections.Generic;
using System.Threading.Tasks;
using SafiStore.Api.Application.DTOs;

namespace SafiStore.Api.Infrastructure.Services
{
    /// <summary>
    /// Interface for Cart business logic
    /// Handles shopping cart operations
    /// </summary>
    public interface ICartService
    {
        // Get user's cart items
        Task<List<CartItemDto>> GetUserCartAsync(int userId);

        // Add product to cart
        Task<SafiStore.Api.Application.DTOs.ServiceResult<CartItemDto>> AddToCartAsync(int userId, int productId, int quantity);

        // Update cart item quantity
        Task<CartItemDto> UpdateCartItemAsync(int userId, int cartItemId, int quantity);

        // Remove item from cart
        Task RemoveFromCartAsync(int userId, int cartItemId);

        // Clear entire cart
        Task ClearCartAsync(int userId);

        // Get cart total
        Task<decimal> GetCartTotalAsync(int userId);
    }
}
