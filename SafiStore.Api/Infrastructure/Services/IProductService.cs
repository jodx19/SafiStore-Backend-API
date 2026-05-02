// Services/IProductService.cs
using SafiStore.Api.Application.DTOs;

namespace SafiStore.Api.Infrastructure.Services
{
    /// <summary>
    /// Interface for Product business logic
    /// Defines all product-related operations
    /// </summary>
    public interface IProductService
    {
        // Get all products with pagination and filtering
        Task<(List<ProductDto> Products, int Total)> GetAllProductsAsync(
            int page = 1,
            int pageSize = 20,
            string? category = null,
            string? searchTerm = null);

        // Get single product by ID
        Task<ProductDto?> GetProductByIdAsync(int id);

        // Get all categories
        Task<List<CategoryDto>> GetCategoriesAsync();

        // Create new product (Admin only)
        Task<ServiceResult<ProductDto>> CreateProductAsync(CreateProductDto dto);

        // Update existing product (Admin only)
        Task<ProductDto> UpdateProductAsync(int id, UpdateProductDto dto);

        // Delete product (Admin only)
        Task DeleteProductAsync(int id);

        // Get products by category
        Task<List<ProductDto>> GetProductsByCategoryAsync(int categoryId);
    }
}
