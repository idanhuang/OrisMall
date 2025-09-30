using Microsoft.EntityFrameworkCore;
using OrisMall.Core.DTOs;
using OrisMall.Core.Entities;
using OrisMall.Core.Interfaces;
using OrisMall.Infrastructure.Data;

namespace OrisMall.Infrastructure.Services;

public class CategoryService : ICategoryService
{
    private readonly ICategoryRepository _categoryRepository;
    private readonly OrisMallDbContext _context;

    public CategoryService(ICategoryRepository categoryRepository, OrisMallDbContext context)
    {
        _categoryRepository = categoryRepository;
        _context = context;
    }

    public async Task<IEnumerable<CategoryDto>> GetAllCategoriesAsync()
    {
        var categories = await _categoryRepository.GetAllAsync();
        var categoryDtos = new List<CategoryDto>();

        foreach (var category in categories)
        {
            var productCount = await _context.Products.CountAsync(p => p.CategoryId == category.Id && p.IsActive);
            categoryDtos.Add(MapToDto(category, productCount));
        }

        return categoryDtos;
    }

    public async Task<CategoryDto?> GetCategoryByIdAsync(int id)
    {
        var category = await _categoryRepository.GetByIdAsync(id);
        if (category == null)
            return null;

        var productCount = await _context.Products.CountAsync(p => p.CategoryId == category.Id && p.IsActive);
        return MapToDto(category, productCount);
    }

    public async Task<CategoryDto> CreateCategoryAsync(CreateCategoryDto createCategoryDto)
    {
        var category = new Category
        {
            Name = createCategoryDto.Name,
            Description = createCategoryDto.Description,
            ImageUrl = createCategoryDto.ImageUrl,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        var createdCategory = await _categoryRepository.AddAsync(category);
        return MapToDto(createdCategory, 0);
    }

    public async Task<CategoryDto?> UpdateCategoryAsync(int id, UpdateCategoryDto updateCategoryDto)
    {
        var category = await _categoryRepository.GetByIdAsync(id);
        if (category == null)
            return null;

        category.Name = updateCategoryDto.Name;
        category.Description = updateCategoryDto.Description;
        category.ImageUrl = updateCategoryDto.ImageUrl;
        category.IsActive = updateCategoryDto.IsActive;
        category.UpdatedAt = DateTime.UtcNow;

        var updatedCategory = await _categoryRepository.UpdateAsync(category);
        var productCount = await _context.Products.CountAsync(p => p.CategoryId == category.Id && p.IsActive);
        return MapToDto(updatedCategory, productCount);
    }

    public async Task<bool> DeleteCategoryAsync(int id)
    {
        // Check if category has products
        var hasProducts = await _context.Products.AnyAsync(p => p.CategoryId == id);
        if (hasProducts)
        {
            throw new InvalidOperationException("Cannot delete category that has products");
        }

        return await _categoryRepository.DeleteAsync(id);
    }

    public async Task<bool> CategoryExistsAsync(int id)
    {
        return await _categoryRepository.ExistsAsync(id);
    }

    private static CategoryDto MapToDto(Category category, int productCount)
    {
        return new CategoryDto
        {
            Id = category.Id,
            Name = category.Name,
            Description = category.Description,
            ImageUrl = category.ImageUrl,
            IsActive = category.IsActive,
            CreatedAt = category.CreatedAt,
            UpdatedAt = category.UpdatedAt,
            ProductCount = productCount
        };
    }
}
