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
        _logger.LogInformation("Retrieving all products");
        
        var products = await _productService.GetAllProductsAsync();
        
        _logger.LogInformation("Retrieved {ProductCount} products successfully", products.Count());
        return Ok(products);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ProductDto>> GetProduct(int id)
    {
        _logger.LogInformation("Retrieving product with ID {ProductId}", id);
        
        var product = await _productService.GetProductByIdAsync(id);
        if (product == null)
        {
            _logger.LogWarning("Product {ProductId} not found", id);
            return NotFound();
        }

        _logger.LogInformation("Product {ProductId} retrieved successfully", id);
        return Ok(product);
    }

    [HttpGet("category/{categoryId}")]
    public async Task<ActionResult<IEnumerable<ProductDto>>> GetProductsByCategory(int categoryId)
    {
        _logger.LogInformation("Retrieving products for category {CategoryId}", categoryId);
        
        var products = await _productService.GetProductsByCategoryAsync(categoryId);
        
        _logger.LogInformation("Retrieved {ProductCount} products for category {CategoryId}", products.Count(), categoryId);
        return Ok(products);
    }

    [HttpGet("filter")]
    public async Task<ActionResult<object>> FilterProducts([FromQuery] string? name, [FromQuery] int? categoryId,
        [FromQuery] decimal? minPrice, [FromQuery] decimal? maxPrice, [FromQuery] bool? inStock,
        [FromQuery] string? sortBy, [FromQuery] string? sortDirection, [FromQuery] int? page, [FromQuery] int? pageSize)
    {
        _logger.LogInformation("Filtering products with parameters: Name={Name}, CategoryId={CategoryId}, MinPrice={MinPrice}, MaxPrice={MaxPrice}, InStock={InStock}, SortBy={SortBy}, SortDirection={SortDirection}, Page={Page}, PageSize={PageSize}", 
            name, categoryId, minPrice, maxPrice, inStock, sortBy, sortDirection, page, pageSize);

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
        
        _logger.LogInformation("Product filtering completed: {ItemCount} items found out of {Total} total", items.Count(), total);
        return Ok(new { total, items });
    }

    [HttpGet("search")]
    public async Task<ActionResult<IEnumerable<ProductDto>>> SearchProducts([FromQuery] string q)
    {
        if (string.IsNullOrWhiteSpace(q))
        {
            _logger.LogWarning("Search request with empty query");
            return BadRequest("Search query is required");
        }

        _logger.LogInformation("Searching products with query: {Query}", q);
        
        var products = await _productService.SearchProductsAsync(q);
        
        _logger.LogInformation("Search completed for query '{Query}': {ProductCount} products found", q, products.Count());
        return Ok(products);
    }

    [HttpPost]
    [Authorize(Policy = "AdminOnly")]
    public async Task<ActionResult<ProductDto>> CreateProduct(CreateProductDto createProductDto)
    {
        _logger.LogInformation("Creating product with name {ProductName}", createProductDto.Name);
        
        var product = await _productService.CreateProductAsync(createProductDto);
        
        _logger.LogInformation("Product created successfully with ID {ProductId}", product.Id);
        return CreatedAtAction(nameof(GetProduct), new { id = product.Id }, product);
    }

    [HttpPut("{id}")]
    [Authorize(Policy = "AdminOnly")]
    public async Task<IActionResult> UpdateProduct(int id, UpdateProductDto updateProductDto)
    {
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