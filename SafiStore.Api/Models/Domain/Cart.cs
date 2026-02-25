using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SafiStore.Api.Models.Domain
{
    public class Cart
    {
        [Key]
        public int Id { get; set; }

        // One Cart per User
        [Required]
        public int UserId { get; set; }

        [StringLength(50)]
        public string Status { get; set; } = "Active";

        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        // Navigation
        public User User { get; set; } = null!;
        public ICollection<CartItem> Items { get; set; } = new List<CartItem>();
    }
}
