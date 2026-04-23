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
        private readonly AppDbContext _context;

        public ReviewService(AppDbContext context)
        {
            _context = context;
        }

        private static ReviewDto MapToDto(Review r)
        {
            var reviewer = string.Join(" ",
                new[] { r.User?.FirstName, r.User?.LastName }
                    .Where(s => !string.IsNullOrWhiteSpace(s))).Trim();

            return new ReviewDto
            {
                Id = r.Id,
                UserId = r.UserId,
                ProductId = r.ProductId,
                ProductName = r.Product?.Title,
                UserName = string.IsNullOrEmpty(reviewer) ? null : reviewer,
                Rating = r.Rating,
                Comment = r.Comment ?? string.Empty,
                CreatedAt = r.CreatedAt
            };
        }

        /// <summary>Returns paginated reviews for a product as <see cref="ReviewDto"/>.</summary>
        public async Task<List<ReviewDto>> GetProductReviewsAsync(int productId, int page = 1, int pageSize = 20)
        {
            var rows = await _context.Reviews
                .AsNoTracking()
                .Include(r => r.Product)
                .Include(r => r.User)
                .Where(r => r.ProductId == productId)
                .OrderByDescending(r => r.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return rows.Select(MapToDto).ToList();
        }

        /// <summary>Adds a review after verifying the user purchased the product.</summary>
        public async Task<ServiceResult<ReviewDto>> AddReviewAsync(int userId, CreateReviewDto dto)
        {
            var hasPurchased = await _context.Orders
                .AnyAsync(o => o.UserId == userId &&
                               o.Items.Any(i => i.ProductId == dto.ProductId));

            if (!hasPurchased)
                return ServiceResult<ReviewDto>.Fail("NOT_PURCHASED", "You can only review products you have purchased.");

            var body = (dto.Comment ?? string.Empty).Trim();
            var title = body.Length == 0
                ? $"Rating {dto.Rating}/5"
                : (body.Length > 200 ? body.Substring(0, 200) : body);
            var comment = body.Length > 2000 ? body.Substring(0, 2000) : body;

            var review = new Review
            {
                UserId = userId,
                ProductId = dto.ProductId,
                Rating = dto.Rating,
                Title = title,
                Comment = comment,
                CreatedAt = DateTime.UtcNow
            };

            _context.Reviews.Add(review);
            await _context.SaveChangesAsync();

            var created = await _context.Reviews
                .AsNoTracking()
                .Include(r => r.Product)
                .Include(r => r.User)
                .FirstAsync(r => r.Id == review.Id);

            return ServiceResult<ReviewDto>.SuccessResult(MapToDto(created), "Review added successfully");
        }

        /// <summary>Updates a review after verifying ownership.</summary>
        public async Task<ServiceResult<ReviewDto>> UpdateReviewAsync(int userId, int reviewId, UpdateReviewDto dto)
        {
            if (!dto.Rating.HasValue && dto.Comment == null)
                return ServiceResult<ReviewDto>.Fail("VALIDATION_ERROR", "Provide rating and/or comment to update.");

            var review = await _context.Reviews
                .Include(r => r.Product)
                .Include(r => r.User)
                .FirstOrDefaultAsync(r => r.Id == reviewId);

            if (review == null)
                return ServiceResult<ReviewDto>.Fail("NOT_FOUND", "Review not found");

            if (review.UserId != userId)
                return ServiceResult<ReviewDto>.Fail("UNAUTHORIZED", "You can only update your own reviews");

            if (dto.Rating.HasValue)
                review.Rating = dto.Rating.Value;

            if (dto.Comment != null)
            {
                var body = dto.Comment.Trim();
                review.Comment = body.Length > 2000 ? body.Substring(0, 2000) : body;
                review.Title = body.Length == 0
                    ? $"Rating {review.Rating}/5"
                    : (body.Length > 200 ? body.Substring(0, 200) : body);
            }

            review.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            var updated = await _context.Reviews
                .AsNoTracking()
                .Include(r => r.Product)
                .Include(r => r.User)
                .FirstAsync(r => r.Id == reviewId);

            return ServiceResult<ReviewDto>.SuccessResult(MapToDto(updated), "Review updated successfully");
        }

        /// <summary>Deletes a review after verifying ownership.</summary>
        public async Task<ServiceResult<bool>> DeleteReviewAsync(int userId, int reviewId)
        {
            var review = await _context.Reviews.FindAsync(reviewId);
            if (review == null)
                return ServiceResult<bool>.Fail("NOT_FOUND", "Review not found");

            if (review.UserId != userId)
                return ServiceResult<bool>.Fail("UNAUTHORIZED", "You can only delete your own reviews");

            _context.Reviews.Remove(review);
            await _context.SaveChangesAsync();
            return ServiceResult<bool>.SuccessResult(true, "Review deleted successfully");
        }

        /// <summary>Gets product review summary (average rating and count).</summary>
        public async Task<ProductReviewSummaryDto> GetProductSummaryAsync(int productId)
        {
            var reviews = await _context.Reviews
                .AsNoTracking()
                .Where(r => r.ProductId == productId)
                .ToListAsync();

            return new ProductReviewSummaryDto
            {
                ProductId = productId,
                TotalReviews = reviews.Count,
                AverageRating = reviews.Count > 0
                    ? Math.Round(reviews.Average(r => r.Rating), 1)
                    : 0
            };
        }

        /// <summary>Gets all reviews by a specific user.</summary>
        public async Task<List<ReviewDto>> GetMyReviewsAsync(int userId)
        {
            var rows = await _context.Reviews
                .AsNoTracking()
                .Include(r => r.Product)
                .Include(r => r.User)
                .Where(r => r.UserId == userId)
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync();

            return rows.Select(MapToDto).ToList();
        }
    }
}
