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
        var categories = await _categoryService.GetAllCategoriesAsync();
        return Ok(categories);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<CategoryDto>> GetCategory(int id)
    {
        var category = await _categoryService.GetCategoryByIdAsync(id);
        if (category == null)
            return NotFound();

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

        try
        {
            _logger.LogInformation("Creating category with name {CategoryName}", createCategoryDto.Name);
            
            var category = await _categoryService.CreateCategoryAsync(createCategoryDto);
            
            _logger.LogInformation("Category created successfully with ID {CategoryId}", category.Id);
            return CreatedAtAction(nameof(GetCategory), new { id = category.Id }, category);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error creating category");
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateCategory(int id, UpdateCategoryDto updateCategoryDto)
    {
        if (!ModelState.IsValid)
        {
            _logger.LogWarning("Invalid category update request model state for category ID {CategoryId}", id);
            return BadRequest(ModelState);
        }

        try
        {
            _logger.LogInformation("Updating category with ID {CategoryId}", id);
            
            await _categoryService.UpdateCategoryAsync(id, updateCategoryDto);
            
            _logger.LogInformation("Category updated successfully with ID {CategoryId}", id);
            return NoContent();
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning("Category update failed for ID {CategoryId}: {Message}", id, ex.Message);
            return BadRequest(ex.Message);
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteCategory(int id)
    {
        try
        {
            await _categoryService.DeleteCategoryAsync(id);
            return NoContent();
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }
}