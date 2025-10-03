using Moq;
using OrisMall.Core.Interfaces;
using OrisMall.Infrastructure.Services;
using OrisMall.Tests.Helpers;

namespace OrisMall.Tests.Services;

public class CategoryServiceTests
{
    private readonly Mock<ICategoryRepository> _mockCategoryRepository;
    private readonly CategoryService _categoryService;

    public CategoryServiceTests()
    {
        _mockCategoryRepository = new Mock<ICategoryRepository>();
        _categoryService = new CategoryService(_mockCategoryRepository.Object);
    }

    [Fact]
    public async Task GetAllCategoriesAsync_ShouldReturnAllCategories_WhenCategoriesExist()
    {
        // Arrange
        var mockCategories = MockDataHelper.GetMockCategories();
        _mockCategoryRepository.Setup(r => r.GetAllAsync()).ReturnsAsync(mockCategories);

        // Act
        var result = await _categoryService.GetAllCategoriesAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(mockCategories.Count, result.Count());
        _mockCategoryRepository.Verify(r => r.GetAllAsync(), Times.Once); // only call one time
    }
}