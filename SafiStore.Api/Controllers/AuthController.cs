using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SafiStore.Api.Application.DTOs;
using SafiStore.Api.Infrastructure.Services;
using System.Threading.Tasks;

namespace SafiStore.Api.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<ActionResult<ApiResponse<LoginResultDto>>> Login([FromBody] LoginDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ApiResponse<LoginResultDto>.Error("VALIDATION_ERROR", "Invalid input", ModelState));

            var result = await _authService.LoginAsync(dto);
            if (result == null)
                return Unauthorized(ApiResponse<LoginResultDto>.Error("AUTH_FAILED", "Invalid credentials"));

            return Ok(ApiResponse<LoginResultDto>.Ok(result, "Login successful"));
        }

        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<ActionResult<ApiResponse<RegisterResultDto>>> Register([FromBody] RegisterDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ApiResponse<RegisterResultDto>.Error("VALIDATION_ERROR", "Invalid input", ModelState));

            var serviceResult = await _authService.RegisterAsync(dto);

            if (!serviceResult.Success)
            {
                // Controllers should never inspect EF exceptions. They only map service-level
                // error codes/messages to ApiResponse objects. This keeps DB logic inside services.
                return BadRequest(ApiResponse<RegisterResultDto>.Error(serviceResult.ErrorCode ?? "REG_FAILED", serviceResult.Message ?? "Registration failed"));
            }

            return Ok(ApiResponse<RegisterResultDto>.Ok(serviceResult.Data!, "Registration successful"));
        }

        [HttpPost("revoke-refresh-token")]
        [Authorize]  // Must be authenticated — prevents anonymous token-revocation attempts
        public async Task<ActionResult<ApiResponse<object>>> RevokeRefreshToken([FromBody] string refreshToken)
        {
            if (string.IsNullOrEmpty(refreshToken))
                return BadRequest(ApiResponse<object>.Error("INVALID_TOKEN", "Refresh token is required"));

            await _authService.RevokeRefreshTokenAsync(refreshToken);
            return Ok(ApiResponse<object>.Ok(null, "Refresh token revoked successfully"));
        }
    }
}
