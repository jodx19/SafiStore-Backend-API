using System.ComponentModel.DataAnnotations;

namespace SafiStore.Api.Application.DTOs
{
    public class UpdateProductDto
    {
        public string? Title { get; set; }
        public string? Description { get; set; }
        [Range(0.01, 999999.99)]
        public decimal? Price { get; set; }
        [Range(0, int.MaxValue)]
        public int? Stock { get; set; }
        public int? CategoryId { get; set; }
        [Url]
        public string? ImageUrl { get; set; }
    }
}
