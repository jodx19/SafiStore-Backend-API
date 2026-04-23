using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SafiStore.Api.Models.Domain
{
    /// <summary>
    /// Order entity - customer orders
    /// Represents a complete purchase transaction
    /// </summary>
    public class Order
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int UserId { get; set; }

        [Required]
        [Range(0.01, 9999999.99)]
        public decimal TotalAmount { get; set; }

        // Pending, Processing, Shipped, Delivered, Cancelled
        [StringLength(50)]
        public string Status { get; set; } = "Pending";

        [Required]
        [StringLength(255)]
        public string ShippingAddress { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string City { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string Country { get; set; } = string.Empty;

        [Required]
        [StringLength(10)]
        public string PostalCode { get; set; } = string.Empty;

        // Optional payment / shipping info
        [StringLength(50)]
        public string? PaymentMethod { get; set; }

        [StringLength(100)]
        public string? TrackingNumber { get; set; }

        // Audit fields
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public DateTime? ShippedAt { get; set; }
        public DateTime? DeliveredAt { get; set; }

        // Navigation
        public ApplicationUser User { get; set; } = null!;
        public ICollection<OrderItem> Items { get; set; } = new List<OrderItem>();
    }
}
