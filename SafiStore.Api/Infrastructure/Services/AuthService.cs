using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SafiStore.Api.Data;
using SafiStore.Api.Common.Extensions;
using SafiStore.Api.Models.Domain;
using SafiStore.Api.Application.DTOs;
using SafiStore.Api.Common.Helpers;

namespace SafiStore.Api.Infrastructure.Services
{
    public class AuthService : IAuthService
    {
        private readonly ApplicationDbContext _context;
        private readonly IJwtService _jwtService;

        public AuthService(ApplicationDbContext context, IJwtService jwtService)
        {
            _context = context;
            _jwtService = jwtService;
        }

        public async Task<LoginResultDto?> LoginAsync(LoginDto dto)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Email == dto.Email);

            if (user == null)
                return null;

            bool isAuthenticated = false;
            bool needsRehash = false;

            // ── Step 1: Try modern PBKDF2 verification ──────────────────────
            var (isValid, rehashNeeded) = PasswordHasher.VerifyWithRehashFlag(dto.Password, user.PasswordHash);

            if (isValid)
            {
                isAuthenticated = true;
                needsRehash = rehashNeeded;
            }
            // ── Step 2: Fallback to legacy SHA256 (migration path) ───────────
            // This block runs ONLY if PBKDF2 verification failed.
            // If the hash looks like a SHA256 hash, we try to verify it,
            // then immediately re-hash it with PBKDF2 and save to DB.
            else if (PasswordHasher.VerifyLegacySha256(dto.Password, user.PasswordHash))
            {
                isAuthenticated = true;
                needsRehash = true; // Always re-hash when migrating from SHA256
            }

            if (!isAuthenticated)
                return null;

            // ── Step 3: Re-hash if needed (identity upgrade or SHA256 migration) ──
            if (needsRehash)
            {
                user.PasswordHash = PasswordHasher.Hash(dto.Password);
                user.UpdatedAt = DateTime.UtcNow;
                await _context.SaveChangesAsync();
            }

            var accessToken = _jwtService.GenerateAccessToken(
                user.Id,
                user.Email,
                user.Role
            );

            var refreshToken = _jwtService.GenerateRefreshToken();
            await SaveRefreshTokenAsync(user.Id, refreshToken);

            return new LoginResultDto
            {
                UserId = user.Id,
                Email = user.Email,
                Role = user.Role,
                AccessToken = accessToken,
                RefreshToken = refreshToken
            };
        }

        public async Task<Application.DTOs.ServiceResult<RegisterResultDto>> RegisterAsync(RegisterDto dto)
        {
            // 1. التأكد من عدم تكرار الإيميل
            if (await _context.Users.AnyAsync(u => u.Email == dto.Email))
            {
                return Application.DTOs.ServiceResult<RegisterResultDto>.Fail("EMAIL_EXISTS", "A user with this email already exists");
            }

            // 2. إنشاء كائن المستخدم ونقل البيانات من الـ DTO (التعديل هنا)
            var user = new User
            {
                FirstName = dto.FirstName, // السطر المعدل
                LastName = dto.LastName,   // السطر المعدل
                Email = dto.Email,
                PasswordHash = PasswordHasher.Hash(dto.Password),
                Role = "User",             // قيمة افتراضية للرتبة
                IsActive = true,           // لضمان أن الحساب يعمل فوراً
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _context.Users.Add(user);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (Microsoft.EntityFrameworkCore.DbUpdateException dbEx)
            {
                // التعامل مع حالات التزامن أو قيود قاعدة البيانات الفريدة
                if (dbEx.IsUniqueConstraintViolation())
                    return Application.DTOs.ServiceResult<RegisterResultDto>.Fail("EMAIL_EXISTS", "A user with this email already exists");

                return Application.DTOs.ServiceResult<RegisterResultDto>.Fail("DB_UPDATE_ERROR", "Unable to complete registration due to database error");
            }

            // 3. تحضير نتيجة الرد
            var result = new RegisterResultDto
            {
                UserId = user.Id,
                Email = user.Email
            };

            return Application.DTOs.ServiceResult<RegisterResultDto>.SuccessResult(result, "User registered successfully");
        }

        public async Task SaveRefreshTokenAsync(int userId, string refreshToken)
        {
            _context.RefreshTokens.Add(new RefreshToken
            {
                UserId = userId,
                Token = refreshToken,
                ExpiresAt = DateTime.UtcNow.AddDays(30)
            });

            await _context.SaveChangesAsync();
        }

        public async Task RevokeRefreshTokenAsync(string refreshToken)
        {
            var token = await _context.RefreshTokens
                .FirstOrDefaultAsync(t => t.Token == refreshToken);

            if (token == null) return;

            _context.RefreshTokens.Remove(token);
            await _context.SaveChangesAsync();
        }
    }
}
