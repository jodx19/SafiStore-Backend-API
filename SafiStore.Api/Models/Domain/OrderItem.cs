using System.ComponentModel.DataAnnotations;

namespace SafiStore.Api.Models.Domain
{
    /// <summary>
    /// OrderItem entity - individual items in an order
    /// </summary>
    public class OrderItem
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int OrderId { get; set; }

        [Required]
        public int ProductId { get; set; }

        [Required]
        [Range(1, 999)]
        public int Quantity { get; set; }

        [Required]
        [Range(0.01, 999999.99)]
        public decimal UnitPrice { get; set; }

        // Navigation
        public Order Order { get; set; } = null!;
        public Product Product { get; set; } = null!;
    }
}
