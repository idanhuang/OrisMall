using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using OrisMall.Core.DTOs;
using OrisMall.Core.Interfaces;

namespace OrisMall.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly IProductService _productService;
    private readonly ILogger<ProductsController> _logger;

    public ProductsController(IProductService productService, ILogger<ProductsController> logger)
    {
        _productService = productService;
        _logger = logger;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<ProductDto>>> GetProducts()
    {
        var products = await _productService.GetAllProductsAsync();
        return Ok(products);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ProductDto>> GetProduct(int id)
    {
        var product = await _productService.GetProductByIdAsync(id);
        if (product == null)
            return NotFound();

        return Ok(product);
    }

    [HttpGet("category/{categoryId}")]
    public async Task<ActionResult<IEnumerable<ProductDto>>> GetProductsByCategory(int categoryId)
    {
        var products = await _productService.GetProductsByCategoryAsync(categoryId);
        return Ok(products);
    }

    [HttpGet("filter")]
    public async Task<ActionResult<object>> FilterProducts([FromQuery] string? name, [FromQuery] int? categoryId,
        [FromQuery] decimal? minPrice, [FromQuery] decimal? maxPrice, [FromQuery] bool? inStock,
        [FromQuery] string? sortBy, [FromQuery] string? sortDirection, [FromQuery] int? page, [FromQuery] int? pageSize)
    {
        var filter = new ProductFilterDto
        {
            Name = name,
            CategoryId = categoryId,
            MinPrice = minPrice,
            MaxPrice = maxPrice,
            InStock = inStock,
            SortBy = sortBy,
            SortDirection = sortDirection,
            Page = page,
            PageSize = pageSize
        };

        var (items, total) = await _productService.FilterProductsAsync(filter);
        return Ok(new { total, items });
    }

    [HttpGet("search")]
    public async Task<ActionResult<IEnumerable<ProductDto>>> SearchProducts([FromQuery] string q)
    {
        if (string.IsNullOrWhiteSpace(q))
            return BadRequest("Search query is required");

        var products = await _productService.SearchProductsAsync(q);
        return Ok(products);
    }

    [HttpPost]
    [Authorize(Policy = "AdminOnly")]
    public async Task<ActionResult<ProductDto>> CreateProduct(CreateProductDto createProductDto)
    {
        if (!ModelState.IsValid)
        {
            _logger.LogWarning("Invalid product creation request model state");
            return BadRequest(ModelState);
        }

        try
        {
            _logger.LogInformation("Creating product with name {ProductName}", createProductDto.Name);
            
            var product = await _productService.CreateProductAsync(createProductDto);
            
            _logger.LogInformation("Product created successfully with ID {ProductId}", product.Id);
            return CreatedAtAction(nameof(GetProduct), new { id = product.Id }, product);
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning("Product creation failed: {Message}", ex.Message);
            return BadRequest(ex.Message);
        }
    }

    [HttpPut("{id}")]
    [Authorize(Policy = "AdminOnly")]
    public async Task<IActionResult> UpdateProduct(int id, UpdateProductDto updateProductDto)
    {
        if (!ModelState.IsValid)
        {
            _logger.LogWarning("Invalid product update request model state for product ID {ProductId}", id);
            return BadRequest(ModelState);
        }

        try
        {
            _logger.LogInformation("Updating product with ID {ProductId}", id);
            
            await _productService.UpdateProductAsync(id, updateProductDto);
            
            _logger.LogInformation("Product updated successfully with ID {ProductId}", id);
            return NoContent();
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning("Product update failed for ID {ProductId}: {Message}", id, ex.Message);
            return BadRequest(ex.Message);
        }
    }

    [HttpDelete("{id}")]
    [Authorize(Policy = "AdminOnly")]
    public async Task<IActionResult> DeleteProduct(int id)
    {
        try
        {
            await _productService.DeleteProductAsync(id);
            return NoContent();
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }
}