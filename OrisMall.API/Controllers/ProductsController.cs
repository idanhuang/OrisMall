using Microsoft.AspNetCore.Mvc;
using OrisMall.Core.DTOs;
using OrisMall.Core.Interfaces;

namespace OrisMall.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly IProductService _productService;

    public ProductsController(IProductService productService)
    {
        _productService = productService;
    }

    /// <summary>
    /// Get all products
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<ProductDto>>> GetProducts()
    {
        var products = await _productService.GetAllProductsAsync();
        return Ok(products);
    }

    /// <summary>
    /// Get product by ID
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<ProductDto>> GetProduct(int id)
    {
        var product = await _productService.GetProductByIdAsync(id);
        if (product == null)
        {
            return NotFound();
        }
        return Ok(product);
    }

    /// <summary>
    /// Get products by category
    /// </summary>
    [HttpGet("category/{categoryId}")]
    public async Task<ActionResult<IEnumerable<ProductDto>>> GetProductsByCategory(int categoryId)
    {
        var products = await _productService.GetProductsByCategoryAsync(categoryId);
        return Ok(products);
    }

    /// <summary>
    /// Search products
    /// </summary>
    [HttpGet("search")]
    public async Task<ActionResult<IEnumerable<ProductDto>>> SearchProducts([FromQuery] string q)
    {
        if (string.IsNullOrWhiteSpace(q))
        {
            return BadRequest("Search query cannot be empty");
        }

        var products = await _productService.SearchProductsAsync(q);
        return Ok(products);
    }

    /// <summary>
    /// Create a new product
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<ProductDto>> CreateProduct(CreateProductDto createProductDto)
    {
        try
        {
            var product = await _productService.CreateProductAsync(createProductDto);
            return CreatedAtAction(nameof(GetProduct), new { id = product.Id }, product);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    /// <summary>
    /// Update a product
    /// </summary>
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateProduct(int id, UpdateProductDto updateProductDto)
    {
        try
        {
            var product = await _productService.UpdateProductAsync(id, updateProductDto);
            if (product == null)
            {
                return NotFound();
            }
            return Ok(product);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    /// <summary>
    /// Delete a product
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteProduct(int id)
    {
        var result = await _productService.DeleteProductAsync(id);
        if (!result)
        {
            return NotFound();
        }
        return NoContent();
    }
}
