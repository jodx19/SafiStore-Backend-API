using System.ComponentModel.DataAnnotations;

namespace SafiStore.Api.Application.DTOs
{
    /// <summary>Request body for adding an item to the cart.</summary>
    public class AddToCartDto
    {
        [Required]
        public int ProductId { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be at least 1.")]
        public int Quantity { get; set; }
    }

    /// <summary>Request body for updating cart item quantity.</summary>
    public class UpdateCartItemDto
    {
        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be at least 1.")]
        public int Quantity { get; set; }
    }
}
