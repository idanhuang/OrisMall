using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using OrisMall.Core.DTOs;
using OrisMall.Core.Interfaces;

namespace OrisMall.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CategoriesController : ControllerBase
{
    private readonly ICategoryService _categoryService;
    private readonly ILogger<CategoriesController> _logger;

    public CategoriesController(ICategoryService categoryService, ILogger<CategoriesController> logger)
    {
        _categoryService = categoryService;
        _logger = logger;
    }

    /// <summary>
    /// Create new category (admin only)
    /// Time Complexity: O(1) for database insert operation
    /// Space Complexity: O(1) for single category creation
    /// </summary>
    /// <returns>Created category information</returns>
    [HttpPost]
    [Authorize(Policy = "AdminOnly")]
    public async Task<ActionResult<CategoryDto>> CreateCategory(CreateCategoryDto createCategoryDto)
    {
        _logger.LogInformation("Creating category with name {CategoryName}", createCategoryDto.Name);
        
        var category = await _categoryService.CreateCategoryAsync(createCategoryDto);
        
        _logger.LogInformation("Category created successfully with ID {CategoryId}", category.Id);
        return CreatedAtAction(nameof(GetCategory), new { id = category.Id }, category);
    }

    /// <summary>
    /// Update existing category (admin only)
    /// Time Complexity: O(1) for database update by primary key
    /// Space Complexity: O(1) for single category update
    /// </summary>
    /// <returns>No content on success</returns>
    [HttpPut("{id}")]
    [Authorize(Policy = "AdminOnly")]
    public async Task<IActionResult> UpdateCategory(int id, UpdateCategoryDto updateCategoryDto)
    {
        _logger.LogInformation("Updating category with ID {CategoryId}", id);
        
        await _categoryService.UpdateCategoryAsync(id, updateCategoryDto);
        
        _logger.LogInformation("Category updated successfully with ID {CategoryId}", id);
        return NoContent();
    }

    /// <summary>
    /// Delete category (admin only)
    /// Time Complexity: O(1) for database delete by primary key
    /// Space Complexity: O(1) for single category deletion
    /// </summary>
    /// <returns>No content on success</returns>
    [HttpDelete("{id}")]
    [Authorize(Policy = "AdminOnly")]
    public async Task<IActionResult> DeleteCategory(int id)
    {
        await _categoryService.DeleteCategoryAsync(id);
        return NoContent();
    }

    /// <summary>
    /// Get all categories
    /// Time Complexity: O(n) where n is the number of categories (with caching: O(1) on cache hit)
    /// Space Complexity: O(n) for storing all category data
    /// </summary>
    /// <returns>List of all categories</returns>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<CategoryDto>>> GetCategories()
    {
        _logger.LogInformation("Retrieving all categories");
        
        var categories = await _categoryService.GetAllCategoriesAsync();
        
        _logger.LogInformation("Retrieved {CategoryCount} categories successfully", categories.Count());
        return Ok(categories);
    }

    /// <summary>
    /// Get category by ID
    /// Time Complexity: O(1) for database lookup by primary key
    /// Space Complexity: O(1) for single category data
    /// </summary>
    /// <returns>Category information</returns>
    [HttpGet("{id}")]
    public async Task<ActionResult<CategoryDto>> GetCategory(int id)
    {
        _logger.LogInformation("Retrieving category with ID {CategoryId}", id);
        
        var category = await _categoryService.GetCategoryByIdAsync(id);
        if (category == null)
        {
            _logger.LogWarning("Category {CategoryId} not found", id);
            return NotFound();
        }

        _logger.LogInformation("Category {CategoryId} retrieved successfully", id);
        return Ok(category);
    }


    /// <summary>
    /// Search categories by name
    /// Time Complexity: O(n) where n is the number of categories (with indexing: O(log n))
    /// Space Complexity: O(m) where m is the number of matching categories
    /// </summary>
    /// <returns>List of matching categories</returns>
    [HttpGet("search")]
    public async Task<ActionResult<IEnumerable<CategoryDto>>> SearchCategories([FromQuery] string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            _logger.LogWarning("Search request with empty category name");
            return BadRequest("Category name is required");
        }

        _logger.LogInformation("Searching categories with name: {Name}", name);
        
        var categories = await _categoryService.GetAllCategoriesAsync();
        var matchingCategories = categories.Where(c => 
            c.Name.Contains(name, StringComparison.OrdinalIgnoreCase));
        
        _logger.LogInformation("Category search completed for name '{Name}': {CategoryCount} categories found", name, matchingCategories.Count());
        return Ok(matchingCategories);
    }
}