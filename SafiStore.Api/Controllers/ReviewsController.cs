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
        public async Task<ActionResult<ApiResponse<List<ReviewSummaryDto>>>> GetProductReviews(
            int productId,
            [FromQuery] int page     = 1,
            [FromQuery] int pageSize = 20)
        {
            if (page < 1) page = 1;
            if (pageSize < 1 || pageSize > 100) pageSize = 20;

            var reviews = await _reviewService.GetProductReviewsAsync(productId, page, pageSize);
            return Ok(ApiResponse<List<ReviewSummaryDto>>.Ok(reviews, "Reviews retrieved successfully"));
        }

        /// <summary>Submit a review for a purchased product. Requires authentication.</summary>
        [HttpPost]
        public async Task<ActionResult<ApiResponse<int>>> AddReview([FromBody] CreateReviewDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ApiResponse<int>.Error("VALIDATION_ERROR", "Invalid review data", ModelState));

            var userId = User.GetUserIdOrThrow();
            var result = await _reviewService.AddReviewAsync(userId, dto);

            if (!result.Success)
                return BadRequest(ApiResponse<int>.Error(result.ErrorCode ?? "REVIEW_ERROR", result.Message ?? "Unable to add review"));

            return Ok(ApiResponse<int>.Ok(result.Data, result.Message ?? "Review added successfully"));
        }
    }
}
