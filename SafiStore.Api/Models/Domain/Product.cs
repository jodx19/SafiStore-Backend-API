using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SafiStore.Api.Models.Domain
{
    /// <summary>
    /// Product entity - represents a product in the catalog
    /// </summary>
    public class Product
    {
        // Primary key
        // DatabaseGeneratedOption.None is REQUIRED because we use Seed Data
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)] // غيرنا None لـ Identity
        public int Id { get; set; }

        // Product title
        [Required(ErrorMessage = "Title is required")]
        [StringLength(200)]
        public string Title { get; set; } = string.Empty;

        // Product description
        [Required(ErrorMessage = "Description is required")]
        [StringLength(2000)]
        public string Description { get; set; } = string.Empty;

        // Product price
        [Required]
        [Range(0.01, 999999.99)]
        public decimal Price { get; set; }

        // Available stock
        [Required]
        [Range(0, int.MaxValue)]
        public int Stock { get; set; }

        // Foreign key to Category
        [Required]
        public int CategoryId { get; set; }

        // Navigation property
        public Category? Category { get; set; }

        // Product image URL
        [Url]
        public string? ImageUrl { get; set; }

        // Product rating (0–5)
        [Range(0, 5)]
        public decimal? Rating { get; set; }

        // Audit fields
        // MUST NOT have DateTime.Now / UtcNow defaults because of Seed Data
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        // Soft delete flag
        public bool IsDeleted { get; set; } = false;

        // Navigation collection - product reviews
        public ICollection<Review> Reviews { get; set; } = new List<Review>();
    }
}
