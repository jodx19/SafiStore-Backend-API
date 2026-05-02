using System.ComponentModel.DataAnnotations;

namespace SafiStore.Api.Application.DTOs
{
    public class CreateProductDto
    {
        [Required]
        [StringLength(200, MinimumLength = 3)]
        public required string Title { get; set; }

        [Required]
        [StringLength(2000, MinimumLength = 10)]
        public required string Description { get; set; }

        [Required]
        [Range(0.01, 999999.99)]
        public decimal Price { get; set; }

        [Required]
        [Range(0, int.MaxValue)]
        public int Stock { get; set; }

        [Required]
        public int CategoryId { get; set; }

        [Url]
        public string? ImageUrl { get; set; }

        public string Status { get; set; } = "Pending";
    }
}
