using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SafiStore.Api.Models.Domain
{
    public class CartItem
    {
        [Key]
        public int Id { get; set; }

        // FK → Cart
        [Required]
        public int CartId { get; set; }

        // FK → Product
        [Required]
        public int ProductId { get; set; }

        [Required]
        [Range(1, 999)]
        public int Quantity { get; set; }

        [Required]
        public decimal PriceAtAddition { get; set; }

        public DateTime AddedAt { get; set; } = DateTime.UtcNow;

        // Navigation Properties
        public Cart Cart { get; set; } = null!;
        public Product Product { get; set; } = null!;
    }
}
