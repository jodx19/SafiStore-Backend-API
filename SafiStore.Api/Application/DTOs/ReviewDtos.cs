using System;
using System.ComponentModel.DataAnnotations;

namespace SafiStore.Api.Application.DTOs
{
    /// <summary>Full review shape returned by the API (aligned with the Angular client).</summary>
    public class ReviewDto
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int ProductId { get; set; }
        public string? ProductName { get; set; }
        public string? UserName { get; set; }
        public int Rating { get; set; }
        public string Comment { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }

    /// <summary>Public read shape for product reviews (anonymous-safe).</summary>
    public class ReviewSummaryDto
    {
        public int    Rating       { get; set; }
        public string Comment      { get; set; } = string.Empty;
        public string ReviewerName { get; set; } = string.Empty;
        public DateTime CreatedAt  { get; set; }
    }

    /// <summary>Write shape for submitting a new review.</summary>
    public class CreateReviewDto
    {
        [Required]
        public int ProductId { get; set; }

        [Range(1, 5, ErrorMessage = "Rating must be between 1 and 5.")]
        public int Rating { get; set; }

        [MaxLength(1000)]
        public string? Comment { get; set; }
    }

    /// <summary>Write shape for updating an existing review.</summary>
    public class UpdateReviewDto
    {
        [Range(1, 5, ErrorMessage = "Rating must be between 1 and 5.")]
        public int? Rating { get; set; }

        [MaxLength(1000)]
        public string? Comment { get; set; }
    }

    /// <summary>Product review summary with average rating and count.</summary>
    public class ProductReviewSummaryDto
    {
        public double AverageRating { get; set; }
        public int TotalReviews { get; set; }
        public int ProductId { get; set; }
    }
}
