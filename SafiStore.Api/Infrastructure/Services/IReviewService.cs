using SafiStore.Api.Application.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SafiStore.Api.Infrastructure.Services
{
    public interface IReviewService
    {
        Task<ServiceResult<int>> AddReviewAsync(int userId, CreateReviewDto dto);

        /// <param name="page">1-based page number.</param>
        /// <param name="pageSize">Results per page (max 100).</param>
        Task<List<ReviewSummaryDto>> GetProductReviewsAsync(int productId, int page = 1, int pageSize = 20);
    }
}
