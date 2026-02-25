using Microsoft.EntityFrameworkCore;
using SafiStore.Api.Application.DTOs;
using SafiStore.Api.Data;

namespace SafiStore.Api.Infrastructure.Services
{
    /// <summary>
    /// Manages user-level admin operations: list all users, delete a user.
    /// </summary>
    public class UserService : IUserService
    {
        private readonly ApplicationDbContext _context;

        public UserService(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>Returns all registered users as lightweight projections.</summary>
        public async Task<(List<UserDto> Users, int Total)> GetAllUsersAsync(int page = 1, int pageSize = 20)
        {
            var query = _context.Users.AsNoTracking();

            int total = await query.CountAsync();

            var users = await query
                .OrderBy(u => u.Email)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(u => new UserDto
                {
                    Id        = u.Id,
                    FirstName = u.FirstName,
                    LastName  = u.LastName,
                    Email     = u.Email,
                    Role      = u.Role,
                    IsActive  = u.IsActive,
                    CreatedAt = u.CreatedAt
                })
                .ToListAsync();

            return (users, total);
        }

        /// <summary>Deletes a user by ID. Throws ArgumentException if not found.</summary>
        public async Task DeleteUserAsync(int userId)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null)
                throw new ArgumentException($"User {userId} not found.");

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
        }
    }
}
