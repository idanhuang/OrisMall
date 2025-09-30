using Microsoft.AspNetCore.Mvc;
using OrisMall.Core.DTOs;
using OrisMall.Core.Interfaces;

namespace OrisMall.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CategoriesController : ControllerBase
{
    private readonly ICategoryService _categoryService;

    public CategoriesController(ICategoryService categoryService)
    {
        _categoryService = categoryService;
    }

    /// <summary>
    /// Get all categories
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<CategoryDto>>> GetCategories()
    {
        var categories = await _categoryService.GetAllCategoriesAsync();
        return Ok(categories);
    }

    /// <summary>
    /// Get category by ID
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<CategoryDto>> GetCategory(int id)
    {
        var category = await _categoryService.GetCategoryByIdAsync(id);
        if (category == null)
        {
            return NotFound();
        }
        return Ok(category);
    }

    /// <summary>
    /// Create a new category
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<CategoryDto>> CreateCategory(CreateCategoryDto createCategoryDto)
    {
        var category = await _categoryService.CreateCategoryAsync(createCategoryDto);
        return CreatedAtAction(nameof(GetCategory), new { id = category.Id }, category);
    }

    /// <summary>
    /// Update a category
    /// </summary>
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateCategory(int id, UpdateCategoryDto updateCategoryDto)
    {
        var category = await _categoryService.UpdateCategoryAsync(id, updateCategoryDto);
        if (category == null)
        {
            return NotFound();
        }
        return Ok(category);
    }

    /// <summary>
    /// Delete a category
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteCategory(int id)
    {
        try
        {
            var result = await _categoryService.DeleteCategoryAsync(id);
            if (!result)
            {
                return NotFound();
            }
            return NoContent();
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }
}
