using System;
using System.Security.Claims;

namespace SafiStore.Api.Common.Extensions
{
    public static class ClaimsPrincipalExtensions
    {
        public static int GetUserIdOrThrow(this ClaimsPrincipal user)
        {
            var idClaim = user.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? user.FindFirst("id")?.Value;
            if (string.IsNullOrWhiteSpace(idClaim))
                throw new UnauthorizedAccessException("User ID not found in token");

            if (!int.TryParse(idClaim, out var id))
                throw new UnauthorizedAccessException("Invalid user ID in token");

            return id;
        }
    }
}
