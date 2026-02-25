using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SafiStore.Api.Application.DTOs
{
    public class OrderItemDto
    {
        [Required]
        public int ProductId { get; set; }

        [Required]
        [Range(1, int.MaxValue)]
        public int Quantity { get; set; }
    }

    public class CreateOrderDto
    {
        [Required]
        public int UserId { get; set; }

        [Required]
        public List<OrderItemDto> Items { get; set; }

        [Required]
        public string ShippingAddress { get; set; }

        [Required]
        public string City { get; set; }

        [Required]
        public string Country { get; set; }

        [Required]
        public string PostalCode { get; set; }

        public string? PaymentMethod { get; set; }
    }

    public class OrderDto
    {
        public int Id { get; set; }
        public int UserId { get; set; }

        // عنوان الشحن
        public string ShippingAddress { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string Country { get; set; } = string.Empty;
        public string PostalCode { get; set; } = string.Empty;

        // الحالة والتواريخ والمبالغ
        public string Status { get; set; } = "Pending";
        public DateTime CreatedAt { get; set; }
        public decimal TotalAmount { get; set; }

        // بعض أجزاء الكود السابق كان يستخدم 'Total' لذا نخلي نسخة متوافقة
        public decimal Total
        {
            get => TotalAmount;
            set => TotalAmount = value;
        }

        // عناصر الطلب (يمكن أن تستخدم OrderItemDto أو OrderItemSimpleDto حسب تعريفك)
        public List<OrderItemDto>? Items { get; set; }
    }
}
