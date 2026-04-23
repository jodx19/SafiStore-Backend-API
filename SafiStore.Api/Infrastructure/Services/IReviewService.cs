using SafiStore.Api.Application.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SafiStore.Api.Infrastructure.Services
{
    public interface IReviewService
    {
        Task<ServiceResult<ReviewDto>> AddReviewAsync(int userId, CreateReviewDto dto);

        /// <param name="page">1-based page number.</param>
        /// <param name="pageSize">Results per page (max 100).</param>
        Task<List<ReviewDto>> GetProductReviewsAsync(int productId, int page = 1, int pageSize = 20);

        /// <summary>Update a review. Only the owner can update.</summary>
        Task<ServiceResult<ReviewDto>> UpdateReviewAsync(int userId, int reviewId, UpdateReviewDto dto);

        /// <summary>Delete a review. Only the owner can delete.</summary>
        Task<ServiceResult<bool>> DeleteReviewAsync(int userId, int reviewId);

        /// <summary>Get product review summary (average rating and count).</summary>
        Task<ProductReviewSummaryDto> GetProductSummaryAsync(int productId);

        /// <summary>Get all reviews by a specific user.</summary>
        Task<List<ReviewDto>> GetMyReviewsAsync(int userId);
    }
}
