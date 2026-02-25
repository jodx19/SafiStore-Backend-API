using System.Security.Claims;

namespace SafiStore.Api.Infrastructure.Services
{
    public interface IJwtService
    {
        string GenerateAccessToken(int userId, string email, string role);
        string GenerateRefreshToken();
        ClaimsPrincipal? ValidateToken(string token);
        int GetUserIdFromToken(string token);
    }
}
