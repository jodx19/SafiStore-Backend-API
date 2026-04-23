// Services/ProductService.cs
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SafiStore.Api.Application.DTOs;
using SafiStore.Api.Data;
using SafiStore.Api.Common.Extensions;
using SafiStore.Api.Models.Domain;

namespace SafiStore.Api.Infrastructure.Services
{
    /// <summary>
    /// Product Service Implementation
    /// Handles all product-related business logic and database operations
    /// </summary>
    public class ProductService : IProductService
    {
        private readonly AppDbContext _context;
        private readonly ILogger<ProductService> _logger;

        public ProductService(AppDbContext context, ILogger<ProductService> logger)
        {
            _context = context;
            _logger = logger;
        }

        /// <summary>
        /// Get all products with pagination, filtering, and search
        ///
        /// EXAMPLE USAGE:
        /// var (products, total) = await productService.GetAllProductsAsync(
        ///     page: 1,
        ///     pageSize: 20,
        ///     category: "Electronics",
        ///     searchTerm: "laptop"
        /// );
        /// </summary>
        public async Task<(List<ProductDto> Products, int Total)> GetAllProductsAsync(
            int page = 1,
            int pageSize = 20,
            string? category = null,
            string? searchTerm = null)
        {   
            try
            {
                // ── Select projection: Only fetch needed columns (no full entity load) ──
                IQueryable<ProductDto> query = _context.Products.AsNoTracking()
                    .Where(p => !p.IsDeleted) // Excluding soft-deleted products
                    .Select(p => new ProductDto
                    {
                        Id          = p.Id,
                        Title       = p.Title,
                        Description = p.Description,
                        Price       = p.Price,
                        Stock       = p.Stock,
                        ImageUrl    = p.ImageUrl,
                        CategoryId  = p.CategoryId,
                        CategoryName = p.Category != null ? p.Category.Name : null
                    });

                // Filter by category if provided
                if (!string.IsNullOrEmpty(category))
                    query = query.Where(p => p.CategoryName != null && p.CategoryName.ToLower().Contains(category.ToLower()));

                // Search by title or description
                if (!string.IsNullOrEmpty(searchTerm))
                {
                    var lower = searchTerm.ToLower();
                    query = query.Where(p =>
                        p.Title.ToLower().Contains(lower) ||
                        (p.Description != null && p.Description.ToLower().Contains(lower)));
                }

                int total = await query.CountAsync();

                var products = await query
                    .OrderBy(p => p.Title)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

                return (products, total);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred in GetAllProductsAsync(page: {Page}, pageSize: {PageSize}, category: {Category}, searchTerm: {SearchTerm})", page, pageSize, category, searchTerm);
                throw;
            }
        }

        /// <summary>
        /// Get single product by ID with full details
        /// </summary>
        public async Task<ProductDto?> GetProductByIdAsync(int id)
        {
            var product = await _context.Products
                .AsNoTracking()
                .Where(p => !p.IsDeleted)
                .Include(p => p.Category)
                .Include(p => p.Reviews)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (product == null)
                return null;

            return MapToDto(product);
        }

        /// <summary>
        /// Get all product categories
        /// Used to populate filter dropdowns
        /// </summary>
        public async Task<List<CategoryDto>> GetCategoriesAsync()
        {
            var categories = await _context.Categories
                .AsNoTracking()
                .OrderBy(c => c.Name)
                .ToListAsync();

            return categories.Select(c => new CategoryDto
            {
                Id = c.Id,
                Name = c.Name
            }).ToList();
        }

        /// <summary>
        /// Create new product (Admin only)
        /// Validates stock and price constraints
        /// </summary>
        public async Task<SafiStore.Api.Application.DTOs.ServiceResult<ProductDto>> CreateProductAsync(CreateProductDto dto)
        {
            // Verify category exists
            var category = await _context.Categories.FindAsync(dto.CategoryId);
            if (category == null)
                return Application.DTOs.ServiceResult<ProductDto>.Fail("INVALID_CATEGORY", "Invalid category ID");

            // Check for simple uniqueness constraint on Title to avoid DB unique index violations
            // (There is no SKU property in the model; using Title as an example unique check.)
            if (await _context.Products.AnyAsync(p => p.Title == dto.Title && p.CategoryId == dto.CategoryId))
                return Application.DTOs.ServiceResult<ProductDto>.Fail("PRODUCT_TITLE_EXISTS", "A product with the same title already exists");

            // Create new product entity
            var product = new Product
            {
                Title = dto.Title,
                Description = dto.Description,
                Price = dto.Price,
                Stock = dto.Stock,
                CategoryId = dto.CategoryId,
                ImageUrl = dto.ImageUrl,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _context.Products.Add(product);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (Microsoft.EntityFrameworkCore.DbUpdateException dbEx) when (dbEx.IsUniqueConstraintViolation())
            {
                // Race condition: map DB unique constraint violation to a service-level failure.
                return Application.DTOs.ServiceResult<ProductDto>.Fail("PRODUCT_TITLE_EXISTS", "A product with the same title already exists");
            }
            catch (Microsoft.EntityFrameworkCore.DbUpdateException)
            {
                return Application.DTOs.ServiceResult<ProductDto>.Fail("PRODUCT_CREATION_FAILED", "Unable to create product");
            }

            // Load category for DTO mapping
            await _context.Entry(product).Reference(p => p.Category).LoadAsync();

            return Application.DTOs.ServiceResult<ProductDto>.SuccessResult(MapToDto(product), "Product created successfully");
        }

        /// <summary>
        /// Update existing product (Admin only)
        /// </summary>
        public async Task<ProductDto> UpdateProductAsync(int id, UpdateProductDto dto)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null)
                throw new ArgumentException("Product not found");

            // Update only provided fields
            if (!string.IsNullOrEmpty(dto.Title))
                product.Title = dto.Title;

            if (!string.IsNullOrEmpty(dto.Description))
                product.Description = dto.Description;

            if (dto.Price.HasValue && dto.Price > 0)
                product.Price = dto.Price.Value;

            if (dto.Stock.HasValue && dto.Stock >= 0)
                product.Stock = dto.Stock.Value;

            if (dto.CategoryId.HasValue)
            {
                var category = await _context.Categories.FindAsync(dto.CategoryId.Value);
                if (category == null)
                    throw new ArgumentException("Invalid category ID");
                product.CategoryId = dto.CategoryId.Value;
            }

            if (!string.IsNullOrEmpty(dto.ImageUrl))
                product.ImageUrl = dto.ImageUrl;

            product.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            await _context.Entry(product).Reference(p => p.Category).LoadAsync();

            return MapToDto(product);
        }

        /// <summary>
        /// Delete product (Admin only)
        /// Also removes from carts and orders
        /// </summary>
        public async Task DeleteProductAsync(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                _logger.LogWarning("DeleteProductAsync failed: Product with ID {Id} not found", id);
                throw new ArgumentException("Product not found");
            }

            // Soft delete: set flag and record removal
            product.IsDeleted = true;
            product.UpdatedAt = DateTime.UtcNow;
            
            _context.Products.Update(product);

            // Note: Carts and Orders should remain as they are for history tracking. 
            // Carts will handle the 'deleted' state via availability checks during checkout.

            await _context.SaveChangesAsync();
            _logger.LogInformation("Product with ID {Id} was soft-deleted successfully", id);
        }

        /// <summary>
        /// Get products by category
        /// </summary>
        public async Task<List<ProductDto>> GetProductsByCategoryAsync(int categoryId)
        {
            var products = await _context.Products
                .AsNoTracking()
                .Where(p => p.CategoryId == categoryId)
                .Include(p => p.Category)
                .ToListAsync();

            return products.Select(MapToDto).ToList();
        }

        /// <summary>
        /// Helper method to map Product entity to ProductDto
        /// Used to hide sensitive data and structure response
        /// </summary>
        private ProductDto MapToDto(Product product)
        {
            return new ProductDto
            {
                Id = product.Id,
                Title = product.Title,
                Description = product.Description,
                Price = product.Price,
                Stock = product.Stock,
                CategoryId = product.CategoryId,
                CategoryName = product.Category?.Name,
                ImageUrl = product.ImageUrl,
                Rating = product.Rating ?? 0,
                CreatedAt = product.CreatedAt,
                UpdatedAt = product.UpdatedAt
            };
        }
    }
}
