using Moq;
using OrisMall.Core.DTOs;
using OrisMall.Core.Entities;
using OrisMall.Core.Interfaces;
using OrisMall.Infrastructure.Services;
using OrisMall.Tests.Helpers;

namespace OrisMall.Tests.Services;

public class CategoryServiceTests
{
    private readonly Mock<ICategoryRepository> _mockCategoryRepository;
    private readonly Mock<IProductRepository> _mockProductRepository;
    private readonly CategoryService _categoryService;

    public CategoryServiceTests()
    {
        _mockCategoryRepository = new Mock<ICategoryRepository>();
        _mockProductRepository = new Mock<IProductRepository>();
        _categoryService = new CategoryService(_mockCategoryRepository.Object, _mockProductRepository.Object);
    }

    #region Most Critical Tests

    [Fact]
    public async Task CreateCategoryAsync_ShouldReturnCategoryDto_WhenValidData()
    {
        // Arrange
        var createCategoryDto = new CreateCategoryDto
        {
            Name = "Electronics",
            Description = "Electronic devices",
            ImageUrl = "https://example.com/electronics.jpg"
        };

        var expectedCategory = new Category
        {
            Id = 1,
            Name = createCategoryDto.Name,
            Description = createCategoryDto.Description,
            ImageUrl = createCategoryDto.ImageUrl,
            CreatedAt = DateTime.UtcNow
        };

        _mockCategoryRepository.Setup(r => r.NameExistsAsync(createCategoryDto.Name)).ReturnsAsync(false);
        _mockCategoryRepository.Setup(r => r.AddAsync(It.IsAny<Category>())).ReturnsAsync(expectedCategory);

        // Act
        var result = await _categoryService.CreateCategoryAsync(createCategoryDto);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(expectedCategory.Id, result.Id);
        Assert.Equal(expectedCategory.Name, result.Name);
        Assert.Equal(expectedCategory.Description, result.Description);
        _mockCategoryRepository.Verify(r => r.NameExistsAsync(createCategoryDto.Name), Times.Once);
        _mockCategoryRepository.Verify(r => r.AddAsync(It.IsAny<Category>()), Times.Once);
    }

    [Fact]
    public async Task CreateCategoryAsync_ShouldThrowArgumentException_WhenNameExists()
    {
        // Arrange
        var createCategoryDto = new CreateCategoryDto
        {
            Name = "Electronics",
            Description = "Electronic devices",
            ImageUrl = "https://example.com/electronics.jpg"
        };

        _mockCategoryRepository.Setup(r => r.NameExistsAsync(createCategoryDto.Name)).ReturnsAsync(true);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<ArgumentException>(() => _categoryService.CreateCategoryAsync(createCategoryDto));
        Assert.Equal("Category name already exists", exception.Message);
        _mockCategoryRepository.Verify(r => r.NameExistsAsync(createCategoryDto.Name), Times.Once);
        _mockCategoryRepository.Verify(r => r.AddAsync(It.IsAny<Category>()), Times.Never);
    }

    [Fact]
    public async Task DeleteCategoryAsync_ShouldDeleteCategory_WhenCategoryExistsAndHasNoProducts()
    {
        // Arrange
        var categoryId = 1;
        var category = new Category
        {
            Id = categoryId,
            Name = "Electronics",
            Description = "Electronic devices",
            CreatedAt = DateTime.UtcNow
        };

        _mockCategoryRepository.Setup(r => r.GetByIdAsync(categoryId)).ReturnsAsync(category);
        _mockProductRepository.Setup(r => r.HasProductsInCategoryAsync(categoryId)).ReturnsAsync(false);
        _mockCategoryRepository.Setup(r => r.DeleteAsync(categoryId)).Returns(Task.CompletedTask);

        // Act
        await _categoryService.DeleteCategoryAsync(categoryId);

        // Assert
        _mockCategoryRepository.Verify(r => r.GetByIdAsync(categoryId), Times.Once);
        _mockProductRepository.Verify(r => r.HasProductsInCategoryAsync(categoryId), Times.Once);
        _mockCategoryRepository.Verify(r => r.DeleteAsync(categoryId), Times.Once);
    }

    [Fact]
    public async Task DeleteCategoryAsync_ShouldThrowArgumentException_WhenCategoryNotFound()
    {
        // Arrange
        var categoryId = 999;
        _mockCategoryRepository.Setup(r => r.GetByIdAsync(categoryId)).ReturnsAsync((Category?)null);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<ArgumentException>(() => _categoryService.DeleteCategoryAsync(categoryId));
        Assert.Equal("Category not found", exception.Message);
        _mockCategoryRepository.Verify(r => r.GetByIdAsync(categoryId), Times.Once);
        _mockProductRepository.Verify(r => r.HasProductsInCategoryAsync(It.IsAny<int>()), Times.Never);
        _mockCategoryRepository.Verify(r => r.DeleteAsync(It.IsAny<int>()), Times.Never);
    }

    [Fact]
    public async Task DeleteCategoryAsync_ShouldThrowInvalidOperationException_WhenCategoryHasProducts()
    {
        // Arrange
        var categoryId = 1;
        var category = new Category
        {
            Id = categoryId,
            Name = "Electronics",
            Description = "Electronic devices",
            CreatedAt = DateTime.UtcNow
        };

        _mockCategoryRepository.Setup(r => r.GetByIdAsync(categoryId)).ReturnsAsync(category);
        _mockProductRepository.Setup(r => r.HasProductsInCategoryAsync(categoryId)).ReturnsAsync(true);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => _categoryService.DeleteCategoryAsync(categoryId));
        Assert.Equal("Cannot delete category that has products", exception.Message);
        _mockCategoryRepository.Verify(r => r.GetByIdAsync(categoryId), Times.Once);
        _mockProductRepository.Verify(r => r.HasProductsInCategoryAsync(categoryId), Times.Once);
        _mockCategoryRepository.Verify(r => r.DeleteAsync(It.IsAny<int>()), Times.Never);
    }

    [Fact]
    public async Task GetCategoryByIdAsync_ShouldReturnCategoryDto_WhenCategoryExists()
    {
        // Arrange
        var categoryId = 1;
        var mockCategory = new Category
        {
            Id = categoryId,
            Name = "Electronics",
            Description = "Electronic devices",
            ImageUrl = "https://example.com/electronics.jpg",
            CreatedAt = DateTime.UtcNow
        };

        _mockCategoryRepository.Setup(r => r.GetByIdAsync(categoryId)).ReturnsAsync(mockCategory);

        // Act
        var result = await _categoryService.GetCategoryByIdAsync(categoryId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(mockCategory.Id, result.Id);
        Assert.Equal(mockCategory.Name, result.Name);
        Assert.Equal(mockCategory.Description, result.Description);
        _mockCategoryRepository.Verify(r => r.GetByIdAsync(categoryId), Times.Once);
    }

    [Fact]
    public async Task GetCategoryByIdAsync_ShouldReturnNull_WhenCategoryNotFound()
    {
        // Arrange
        var categoryId = 999;
        _mockCategoryRepository.Setup(r => r.GetByIdAsync(categoryId)).ReturnsAsync((Category?)null);

        // Act
        var result = await _categoryService.GetCategoryByIdAsync(categoryId);

        // Assert
        Assert.Null(result);
        _mockCategoryRepository.Verify(r => r.GetByIdAsync(categoryId), Times.Once);
    }

    #endregion
}