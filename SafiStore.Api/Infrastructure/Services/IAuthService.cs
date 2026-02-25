using SafiStore.Api.Application.DTOs;
using SafiStore.Api.Models.Domain;

namespace SafiStore.Api.Infrastructure.Services
{
    public interface IAuthService
    {
        Task<LoginResultDto?> LoginAsync(LoginDto dto);
        // Return ServiceResult to indicate duplicate email or other expected failures
        Task<SafiStore.Api.Application.DTOs.ServiceResult<RegisterResultDto>> RegisterAsync(RegisterDto dto);

        Task SaveRefreshTokenAsync(int userId, string refreshToken);
        Task RevokeRefreshTokenAsync(string refreshToken);
    }
}
