using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SafiStore.Api.Application.DTOs;
using SafiStore.Api.Infrastructure.Services;
using System;
using System.Threading.Tasks;

namespace SafiStore.Api.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class ProductsController : ControllerBase
    {
        private readonly IProductService _productService;

        public ProductsController(IProductService productService)
        {
            _productService = productService;
        }

        [HttpGet]
        public async Task<ActionResult<ApiResponse<object>>> GetAll(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20,
            [FromQuery] string? category = null,
            [FromQuery] string? search = null)
        {
            if (page < 1) page = 1;
            if (pageSize < 1 || pageSize > 100) pageSize = 20;

            var (products, total) = await _productService.GetAllProductsAsync(page, pageSize, category, search);

            var payload = new
            {
                products,
                pagination = new
                {
                    page,
                    pageSize,
                    total,
                    totalPages = (int)Math.Ceiling((double)total / pageSize)
                }
            };

            return Ok(ApiResponse<object>.Ok(payload, "Products retrieved successfully"));
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<ApiResponse<ProductDto>>> GetById(int id)
        {
            if (id <= 0)
                return BadRequest(ApiResponse<ProductDto>.Error("INVALID_ID", "Invalid product ID"));

            var product = await _productService.GetProductByIdAsync(id);
            if (product == null)
                return NotFound(ApiResponse<ProductDto>.Error("NOT_FOUND", "Product not found"));

            return Ok(ApiResponse<ProductDto>.Ok(product, "Product retrieved successfully"));
        }

        [HttpGet("categories")]
        public async Task<ActionResult<ApiResponse<CategoryDto[]>>> GetCategories()
        {
            var categoriesList = await _productService.GetCategoriesAsync(); 
            var categoriesArray = categoriesList.ToArray(); 

            return Ok(ApiResponse<CategoryDto[]>.Ok(categoriesArray, "Categories retrieved successfully"));
        }


        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<ApiResponse<ProductDto>>> Create([FromBody] CreateProductDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ApiResponse<ProductDto>.Error("VALIDATION_ERROR", "Invalid product data", ModelState));

            var result = await _productService.CreateProductAsync(dto);

            if (!result.Success)
                return BadRequest(ApiResponse<ProductDto>.Error(result.ErrorCode ?? "PRODUCT_CREATION_FAILED", result.Message ?? "Unable to create product"));

            var product = result.Data!;
            return CreatedAtAction(nameof(GetById), new { id = product.Id }, ApiResponse<ProductDto>.Ok(product, result.Message ?? "Product created successfully"));
        }

        [HttpPut("{id:int}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<ApiResponse<ProductDto>>> Update(int id, [FromBody] UpdateProductDto dto)
        {
            if (id <= 0)
                return BadRequest(ApiResponse<ProductDto>.Error("INVALID_ID", "Invalid product ID"));

            if (!ModelState.IsValid)
                return BadRequest(ApiResponse<ProductDto>.Error("VALIDATION_ERROR", "Invalid product data", ModelState));

            try
            {
                var product = await _productService.UpdateProductAsync(id, dto);
                return Ok(ApiResponse<ProductDto>.Ok(product, "Product updated successfully"));
            }
            catch (ArgumentException ex)
            {
                return NotFound(ApiResponse<ProductDto>.Error("NOT_FOUND", ex.Message));
            }
        }

        [HttpDelete("{id:int}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<ApiResponse<object>>> Delete(int id)
        {
            if (id <= 0)
                return BadRequest(ApiResponse<object>.Error("INVALID_ID", "Invalid product ID"));

            try
            {
                await _productService.DeleteProductAsync(id);
                return Ok(ApiResponse<object>.Ok(null, "Product deleted successfully"));
            }
            catch (ArgumentException)
            {
                return NotFound(ApiResponse<object>.Error("NOT_FOUND", "Product not found"));
            }
        }
    }
}
