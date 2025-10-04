using Microsoft.AspNetCore.Mvc;
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

    [HttpGet]
    public async Task<ActionResult<IEnumerable<CategoryDto>>> GetCategories()
    {
        _logger.LogInformation("Retrieving all categories");
        
        var categories = await _categoryService.GetAllCategoriesAsync();
        
        _logger.LogInformation("Retrieved {CategoryCount} categories successfully", categories.Count());
        return Ok(categories);
    }

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

    [HttpPost]
    public async Task<ActionResult<CategoryDto>> CreateCategory(CreateCategoryDto createCategoryDto)
    {
        if (!ModelState.IsValid)
        {
            _logger.LogWarning("Invalid category creation request model state");
            return BadRequest(ModelState);
        }

        _logger.LogInformation("Creating category with name {CategoryName}", createCategoryDto.Name);
        
        var category = await _categoryService.CreateCategoryAsync(createCategoryDto);
        
        _logger.LogInformation("Category created successfully with ID {CategoryId}", category.Id);
        return CreatedAtAction(nameof(GetCategory), new { id = category.Id }, category);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateCategory(int id, UpdateCategoryDto updateCategoryDto)
    {
        if (!ModelState.IsValid)
        {
            _logger.LogWarning("Invalid category update request model state for category ID {CategoryId}", id);
            return BadRequest(ModelState);
        }

        _logger.LogInformation("Updating category with ID {CategoryId}", id);
        
        await _categoryService.UpdateCategoryAsync(id, updateCategoryDto);
        
        _logger.LogInformation("Category updated successfully with ID {CategoryId}", id);
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteCategory(int id)
    {
        await _categoryService.DeleteCategoryAsync(id);
        return NoContent();
    }
}