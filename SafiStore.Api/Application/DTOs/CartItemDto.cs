using System;

namespace SafiStore.Api.Application.DTOs
{
    public class CartItemDto
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public string ProductTitle { get; set; } = string.Empty;
        public string ProductImage { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public decimal PriceAtAddition { get; set; }
        public decimal TotalPrice { get; set; }
        public DateTimeOffset AddedAt { get; set; }
    }
}
