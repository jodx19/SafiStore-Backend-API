using Microsoft.EntityFrameworkCore;
using SafiStore.Api.Application.DTOs;
using SafiStore.Api.Data;
using SafiStore.Api.Models.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SafiStore.Api.Infrastructure.Services
{
    public class ReviewService : IReviewService
    {
        private readonly ApplicationDbContext _context;

        public ReviewService(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>Returns paginated, strongly-typed reviews for a product.</summary>
        public async Task<List<ReviewSummaryDto>> GetProductReviewsAsync(int productId, int page = 1, int pageSize = 20)
        {
            return await _context.Reviews
                .AsNoTracking()
                .Where(r => r.ProductId == productId)
                .OrderByDescending(r => r.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(r => new ReviewSummaryDto
                {
                    Rating       = r.Rating,
                    Comment      = r.Comment ?? string.Empty,
                    ReviewerName = r.User.FirstName + " " + r.User.LastName,
                    CreatedAt    = r.CreatedAt
                })
                .ToListAsync();
        }

        /// <summary>Adds a review after verifying the user purchased the product.</summary>
        public async Task<ServiceResult<int>> AddReviewAsync(int userId, CreateReviewDto dto)
        {
            // Verify the user actually purchased this product
            var hasPurchased = await _context.Orders
                .AnyAsync(o => o.UserId == userId &&
                               o.Items.Any(i => i.ProductId == dto.ProductId));

            if (!hasPurchased)
                return ServiceResult<int>.Fail("NOT_PURCHASED", "You can only review products you have purchased.");

            var review = new Review
            {
                UserId    = userId,
                ProductId = dto.ProductId,
                Rating    = dto.Rating,
                Comment   = dto.Comment,
                CreatedAt = DateTime.UtcNow
            };

            _context.Reviews.Add(review);
            await _context.SaveChangesAsync();

            return ServiceResult<int>.SuccessResult(review.Id, "Review added successfully");
        }
    }
}
