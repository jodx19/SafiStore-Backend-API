using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SafiStore.Api.Application.DTOs;
using SafiStore.Api.Common.Extensions;
using SafiStore.Api.Infrastructure.Services;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SafiStore.Api.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/v1/[controller]")]
    public class ReviewsController : ControllerBase
    {
        private readonly IReviewService _reviewService;

        public ReviewsController(IReviewService reviewService)
        {
            _reviewService = reviewService;
        }

        /// <summary>Get paginated reviews for a specific product. Public — no auth required.</summary>
        [HttpGet("product/{productId:int}")]
        [AllowAnonymous]
        public async Task<ActionResult<ApiResponse<List<ReviewDto>>>> GetProductReviews(
            int productId,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20)
        {
            if (page < 1) page = 1;
            if (pageSize < 1 || pageSize > 100) pageSize = 20;

            var reviews = await _reviewService.GetProductReviewsAsync(productId, page, pageSize);
            return Ok(ApiResponse<List<ReviewDto>>.Ok(reviews, "Reviews retrieved successfully"));
        }

        /// <summary>Submit a review for a purchased product. Requires authentication.</summary>
        [HttpPost]
        public async Task<ActionResult<ApiResponse<ReviewDto>>> AddReview([FromBody] CreateReviewDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ApiResponse<ReviewDto>.Error("VALIDATION_ERROR", "Invalid review data", ModelState));

            var userId = User.GetUserIdOrThrow();
            var result = await _reviewService.AddReviewAsync(userId, dto);

            if (!result.Success || result.Data == null)
                return BadRequest(ApiResponse<ReviewDto>.Error(result.ErrorCode ?? "REVIEW_ERROR", result.Message ?? "Unable to add review"));

            return Ok(ApiResponse<ReviewDto>.Ok(result.Data, result.Message ?? "Review added successfully"));
        }

        /// <summary>Update a review. Only the owner can update.</summary>
        [HttpPut("{reviewId:int}")]
        public async Task<ActionResult<ApiResponse<ReviewDto>>> UpdateReview(
            int reviewId,
            [FromBody] UpdateReviewDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ApiResponse<ReviewDto>.Error("VALIDATION_ERROR", "Invalid review data", ModelState));

            var userId = User.GetUserIdOrThrow();
            var result = await _reviewService.UpdateReviewAsync(userId, reviewId, dto);

            if (!result.Success || result.Data == null)
                return BadRequest(ApiResponse<ReviewDto>.Error(result.ErrorCode ?? "REVIEW_ERROR", result.Message ?? "Unable to update review"));

            return Ok(ApiResponse<ReviewDto>.Ok(result.Data, result.Message ?? "Review updated successfully"));
        }

        /// <summary>Delete a review. Only the owner can delete.</summary>
        [HttpDelete("{reviewId:int}")]
        public async Task<ActionResult<ApiResponse<bool>>> DeleteReview(int reviewId)
        {
            var userId = User.GetUserIdOrThrow();
            var result = await _reviewService.DeleteReviewAsync(userId, reviewId);

            if (!result.Success)
                return BadRequest(ApiResponse<bool>.Error(result.ErrorCode ?? "REVIEW_ERROR", result.Message ?? "Unable to delete review"));

            return Ok(ApiResponse<bool>.Ok(result.Data, result.Message ?? "Review deleted successfully"));
        }

        /// <summary>Get product review summary (average rating and count). Public endpoint.</summary>
        [HttpGet("product/{productId:int}/summary")]
        [AllowAnonymous]
        public async Task<ActionResult<ApiResponse<ProductReviewSummaryDto>>> GetProductSummary(int productId)
        {
            var summary = await _reviewService.GetProductSummaryAsync(productId);
            return Ok(ApiResponse<ProductReviewSummaryDto>.Ok(summary, "Product summary retrieved successfully"));
        }

        /// <summary>Get all reviews by the current user.</summary>
        [HttpGet("my-reviews")]
        public async Task<ActionResult<ApiResponse<List<ReviewDto>>>> GetMyReviews()
        {
            var userId = User.GetUserIdOrThrow();
            var reviews = await _reviewService.GetMyReviewsAsync(userId);
            return Ok(ApiResponse<List<ReviewDto>>.Ok(reviews, "My reviews retrieved successfully"));
        }
    }
}
