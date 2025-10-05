using OrisMall.Core.DTOs;
using OrisMall.Core.Entities;
using OrisMall.Core.Interfaces;

namespace OrisMall.Infrastructure.Services;

public class CategoryService : ICategoryService
{
    private readonly ICategoryRepository _categoryRepository;
    private readonly IProductRepository _productRepository;

    public CategoryService(ICategoryRepository categoryRepository, IProductRepository productRepository)
    {
        _categoryRepository = categoryRepository;
        _productRepository = productRepository;
    }

    public async Task<IEnumerable<CategoryDto>> GetAllCategoriesAsync()
    {
        var categories = await _categoryRepository.GetAllAsync();
        return categories.Select(MapToDto);
    }

    public async Task<CategoryDto?> GetCategoryByIdAsync(int id)
    {
        var category = await _categoryRepository.GetByIdAsync(id);
        return category != null ? MapToDto(category) : null;
    }

    public async Task<CategoryDto> CreateCategoryAsync(CreateCategoryDto createCategoryDto)
    {
        // Check if category name already exists
        if (await _categoryRepository.NameExistsAsync(createCategoryDto.Name))
            throw new ArgumentException("Category name already exists");

        var category = new Category
        {
            Name = createCategoryDto.Name,
            Description = createCategoryDto.Description,
            ImageUrl = createCategoryDto.ImageUrl
        };

        var createdCategory = await _categoryRepository.AddAsync(category);
        return MapToDto(createdCategory);
    }

    public async Task<CategoryDto> UpdateCategoryAsync(int id, UpdateCategoryDto updateCategoryDto)
    {
        var category = await _categoryRepository.GetByIdAsync(id);
        if (category == null)
            throw new ArgumentException("Category not found");

        category.Name = updateCategoryDto.Name;
        category.Description = updateCategoryDto.Description;
        category.ImageUrl = updateCategoryDto.ImageUrl;

        var updatedCategory = await _categoryRepository.UpdateAsync(category);
        return MapToDto(updatedCategory);
    }

    public async Task DeleteCategoryAsync(int id)
    {
        // Check if category exists
        var category = await _categoryRepository.GetByIdAsync(id);
        if (category == null)
            throw new ArgumentException("Category not found");

        // Check if category has products
        if (await _productRepository.HasProductsInCategoryAsync(id))
            throw new InvalidOperationException("Cannot delete category that has products");

        await _categoryRepository.DeleteAsync(id);
    }

    private static CategoryDto MapToDto(Category category)
    {
        return new CategoryDto
        {
            Id = category.Id,
            Name = category.Name,
            Description = category.Description,
            ImageUrl = category.ImageUrl,
            CreatedAt = category.CreatedAt
        };
    }
}




