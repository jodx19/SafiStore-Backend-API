using System.ComponentModel.DataAnnotations;

namespace SafiStore.Api.Common.Helpers
{
    /// <summary>
    /// Strongly-typed JWT configuration.
    /// Validated at startup — if Jwt:Secret is missing or too short, the app fails to start.
    /// </summary>
    public class JwtSettings : IValidatableObject
    {
        [Required]
        public string Secret { get; set; } = string.Empty;

        [Required]
        public string Issuer { get; set; } = string.Empty;

        [Required]
        public string Audience { get; set; } = string.Empty;

        /// <summary>Access token lifetime in minutes. Default: 15.</summary>
        [Range(1, 1440)]
        public int AccessTokenExpiration { get; set; } = 15;

        /// <summary>Refresh token lifetime in days. Default: 30.</summary>
        [Range(1, 365)]
        public int RefreshTokenExpiration { get; set; } = 30;

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            // HS256 requires minimum 32 bytes (256 bits)
            if (string.IsNullOrWhiteSpace(Secret) || Secret.Length < 32)
            {
                yield return new ValidationResult(
                    "Jwt:Secret must be at least 32 characters long for HS256.",
                    new[] { nameof(Secret) });
            }
        }
    }
}
