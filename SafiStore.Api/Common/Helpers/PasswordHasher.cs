using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Identity;
using SafiStore.Api.Models.Domain;

namespace SafiStore.Api.Common.Helpers
{
    /// <summary>
    /// Production-safe password hasher using ASP.NET Core Identity's PBKDF2 implementation.
    /// Uses PasswordHasher&lt;User&gt; (not object) to avoid cross-instance incompatibility.
    /// Also provides a legacy SHA256 verification path for backward compatibility during migration.
    /// </summary>
    public static class PasswordHasher
    {
        private static readonly PasswordHasher<User> _hasher = new PasswordHasher<User>();

        /// <summary>
        /// Hashes a plaintext password using PBKDF2 (Identity V3 format).
        /// </summary>
        public static string Hash(string password)
        {
            return _hasher.HashPassword(null!, password);
        }

        /// <summary>
        /// Verifies a plaintext password against a stored PBKDF2 hash.
        /// Returns true if valid. Also returns whether a rehash is needed
        /// (e.g., algorithm upgrade).
        /// </summary>
        public static (bool IsValid, bool NeedsRehash) VerifyWithRehashFlag(string password, string hash)
        {
            var result = _hasher.VerifyHashedPassword(null!, hash, password);
            return result switch
            {
                PasswordVerificationResult.Success            => (true, false),
                PasswordVerificationResult.SuccessRehashNeeded => (true, true),
                _                                            => (false, false)
            };
        }

        /// <summary>
        /// Simple bool verify. Use VerifyWithRehashFlag in AuthService for migration support.
        /// </summary>
        public static bool Verify(string password, string hash)
        {
            var (isValid, _) = VerifyWithRehashFlag(password, hash);
            return isValid;
        }

        // ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
        // LEGACY SHA256 MIGRATION SUPPORT
        // Used ONLY on login to upgrade old hashes. Remove after
        // all users have logged in once (check DB for SHA256 format).
        // SHA256 hash format: 44-char Base64 string (256 bits → Base64).
        // ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

        private static bool IsLegacySha256Hash(string hash)
        {
            // PBKDF2 hashes start with 'A' (base64 of 0x00 version byte).
            // SHA256 hashes are exactly 44 chars and don't start with 'A' typically.
            // More reliable: try to detect by length (SHA256 → 32 bytes → 44 base64 chars).
            return hash.Length == 44 && !hash.StartsWith("A");
        }

        public static bool VerifyLegacySha256(string password, string sha256Hash)
        {
            using var sha = SHA256.Create();
            var bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(password));
            return Convert.ToBase64String(bytes) == sha256Hash;
        }
    }
}
