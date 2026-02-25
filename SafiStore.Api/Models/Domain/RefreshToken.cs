using System;
using System.ComponentModel.DataAnnotations;

namespace SafiStore.Api.Models.Domain
{
    /// <summary>
    /// RefreshToken entity - stores refresh tokens for JWT authentication
    /// </summary>
    public class RefreshToken
    {
        [Key]
        public int Id { get; set; }
        
        [Required]
        public int UserId { get; set; }

        [Required]
        [StringLength(500)]
        public string Token { get; set; } = string.Empty;

        [Required]
        public DateTime ExpiresAt { get; set; }

        public DateTime CreatedAt { get; set; }

        public bool IsRevoked { get; set; } = false;

        // Navigation
        public User User { get; set; } = null!;
    }
}
