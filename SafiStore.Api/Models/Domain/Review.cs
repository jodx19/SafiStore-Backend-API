using System;
using System.ComponentModel.DataAnnotations;

namespace SafiStore.Api.Models.Domain
{
    /// <summary>
    /// Review entity - customer product reviews
    /// </summary>
    public class Review
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int ProductId { get; set; }

        [Required]
        public int UserId { get; set; }

        [Required]
        [Range(1, 5)]
        public int Rating { get; set; }

        [Required]
        [StringLength(200)]
        public string Title { get; set; } = string.Empty;

        [Required]
        [StringLength(2000)]
        public string Comment { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }

        // Navigation
        public Product Product { get; set; } = null!;
        public ApplicationUser User { get; set; } = null!;
    }
}
