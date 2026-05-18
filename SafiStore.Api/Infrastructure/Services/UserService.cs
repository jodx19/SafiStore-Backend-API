using Microsoft.EntityFrameworkCore;
using SafiStore.Api.Application.DTOs;
using SafiStore.Api.Data;

namespace SafiStore.Api.Infrastructure.Services
{
    public class UserService : IUserService
    {
        private readonly AppDbContext _context;

        public UserService(AppDbContext context)
        {
            _context = context;
        }

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
                    Email     = u.Email!,
                    IsActive  = u.IsActive,
                    CreatedAt = u.CreatedAt
                })
                .ToListAsync();

            return (users, total);
        }

        public async Task DeleteUserAsync(int userId)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null)
                throw new ArgumentException($"User {userId} not found.");

            // Check for existing orders
            var hasOrders = await _context.Orders.AnyAsync(o => o.UserId == userId);
            if (hasOrders)
                throw new InvalidOperationException(
                    $"Cannot delete user {userId}: user has existing orders. " +
                    "Please cancel all orders before deleting the user.");

            // Check for existing reviews
            var hasReviews = await _context.Reviews.AnyAsync(r => r.UserId == userId);
            if (hasReviews)
                throw new InvalidOperationException(
                    $"Cannot delete user {userId}: user has existing reviews. " +
                    "Please remove all reviews before deleting the user.");

            // Check for cart items
            var cartItems = await _context.CartItems
                .Where(ci => ci.Cart!.UserId == userId)
                .ToListAsync();
            if (cartItems.Count != 0)
            {
                _context.CartItems.RemoveRange(cartItems);
            }

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
        }
    }
}
