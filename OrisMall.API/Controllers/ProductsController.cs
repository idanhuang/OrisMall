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

    /// <summary>
    /// Get all products
    /// Time Complexity: O(n) where n is the number of products (with caching: O(1) on cache hit)
    /// Space Complexity: O(n) for storing all product data
    /// </summary>
    /// <returns>List of all products</returns>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<ProductDto>>> GetProducts()
    {
        _logger.LogInformation("Retrieving all products");
        
        var products = await _productService.GetAllProductsAsync();
        
        _logger.LogInformation("Retrieved {ProductCount} products successfully", products.Count());
        return Ok(products);
    }

    /// <summary>
    /// Get product by ID
    /// Time Complexity: O(1) for database lookup by primary key (with caching: O(1) on cache hit)
    /// Space Complexity: O(1) for single product data
    /// </summary>
    /// <returns>Product information</returns>
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

    /// <summary>
    /// Get products by category ID
    /// Time Complexity: O(n) where n is the number of products in the category (with caching: O(1) on cache hit)
    /// Space Complexity: O(n) for storing category products data
    /// </summary>
    /// <returns>List of products in the specified category</returns>
    [HttpGet("category/{categoryId}")]
    public async Task<ActionResult<IEnumerable<ProductDto>>> GetProductsByCategory(int categoryId)
    {
        _logger.LogInformation("Retrieving products for category {CategoryId}", categoryId);
        
        var products = await _productService.GetProductsByCategoryAsync(categoryId);
        
        _logger.LogInformation("Retrieved {ProductCount} products for category {CategoryId}", products.Count(), categoryId);
        return Ok(products);
    }

    /// <summary>
    /// Filter products with various criteria
    /// Time Complexity: O(n) where n is the number of products (with database indexing: O(log n) for sorted queries)
    /// Space Complexity: O(pageSize) for paginated results
    /// </summary>
    /// <returns>Filtered products with pagination</returns>
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

    /// <summary>
    /// Search products by name or description
    /// Time Complexity: O(n) where n is the number of products (with full-text search indexing: O(log n))
    /// Space Complexity: O(m) where m is the number of matching products
    /// </summary>
    /// <returns>List of matching products</returns>
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

    /// <summary>
    /// Create new product (admin only)
    /// Time Complexity: O(1) for database insert operation
    /// Space Complexity: O(1) for single product creation
    /// </summary>
    /// <returns>Created product information</returns>
    [HttpPost]
    [Authorize(Policy = "AdminOnly")]
    public async Task<ActionResult<ProductDto>> CreateProduct(CreateProductDto createProductDto)
    {
        _logger.LogInformation("Creating product with name {ProductName}", createProductDto.Name);
        
        var product = await _productService.CreateProductAsync(createProductDto);
        
        _logger.LogInformation("Product created successfully with ID {ProductId}", product.Id);
        return CreatedAtAction(nameof(GetProduct), new { id = product.Id }, product);
    }

    /// <summary>
    /// Update existing product (admin only)
    /// Time Complexity: O(1) for database update by primary key
    /// Space Complexity: O(1) for single product update
    /// </summary>
    /// <returns>No content on success</returns>
    [HttpPut("{id}")]
    [Authorize(Policy = "AdminOnly")]
    public async Task<IActionResult> UpdateProduct(int id, UpdateProductDto updateProductDto)
    {
        _logger.LogInformation("Updating product with ID {ProductId}", id);
        
        await _productService.UpdateProductAsync(id, updateProductDto);
        
        _logger.LogInformation("Product updated successfully with ID {ProductId}", id);
        return NoContent();
    }

    /// <summary>
    /// Delete product (admin only)
    /// Time Complexity: O(1) for database delete by primary key
    /// Space Complexity: O(1) for single product deletion
    /// </summary>
    /// <returns>No content on success</returns>
    [HttpDelete("{id}")]
    [Authorize(Policy = "AdminOnly")]
    public async Task<IActionResult> DeleteProduct(int id)
    {
        _logger.LogInformation("Deleting product with ID {ProductId}", id);
        
        await _productService.DeleteProductAsync(id);
        
        _logger.LogInformation("Product deleted successfully with ID {ProductId}", id);
        return NoContent();
    }
}