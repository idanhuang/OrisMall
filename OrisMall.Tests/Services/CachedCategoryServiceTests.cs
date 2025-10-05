using Microsoft.Extensions.Caching.Memory;
using Moq;
using OrisMall.Core.Interfaces;
using OrisMall.Infrastructure.Services;
using OrisMall.Tests.Helpers;

namespace OrisMall.Tests.Services;

public class CachedCategoryServiceTests : IDisposable
{
    private readonly Mock<ICategoryService> _mockCategoryService;
    private readonly IMemoryCache _memoryCache;
    private readonly CachedCategoryService _cachedCategoryService;

    public CachedCategoryServiceTests()
    {
        _mockCategoryService = new Mock<ICategoryService>();
        _memoryCache = new MemoryCache(new MemoryCacheOptions());
        _cachedCategoryService = new CachedCategoryService(_mockCategoryService.Object, _memoryCache);
    }

    [Fact]
    public async Task GetAllCategoriesAsync_ShouldReturnCachedData_WhenCacheHit()
    {
        // Arrange
        var mockCategories = MockDataHelper.GetMockCategoryDtos();
        _memoryCache.Set("categories:all", mockCategories); // Pre-populate cache

        // Act
        var result = await _cachedCategoryService.GetAllCategoriesAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(mockCategories.Count, result.Count());
        _mockCategoryService.Verify(s => s.GetAllCategoriesAsync(), Times.Never); // Should not call underlying service
    }

    [Fact]
    public async Task GetAllCategoriesAsync_ShouldCallUnderlyingService_WhenCacheMiss()
    {
        // Arrange
        var mockCategories = MockDataHelper.GetMockCategoryDtos();
        _mockCategoryService.Setup(s => s.GetAllCategoriesAsync()).ReturnsAsync(mockCategories);

        // Act
        var result = await _cachedCategoryService.GetAllCategoriesAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(mockCategories.Count, result.Count());
        
        // Verify that the underlying service WAS called
        _mockCategoryService.Verify(s => s.GetAllCategoriesAsync(), Times.Once);
        
        // Verify that data was cached
        var cachedData = _memoryCache.Get("categories:all");
        Assert.NotNull(cachedData);
    }

    [Fact]
    public async Task GetCategoryByIdAsync_ShouldCallUnderlyingService_WhenCacheMiss()
    {
        // Arrange
        var categoryId = 1;
        var mockCategory = MockDataHelper.GetMockCategoryDtos().First();
        _mockCategoryService.Setup(s => s.GetCategoryByIdAsync(categoryId)).ReturnsAsync(mockCategory);

        // Act
        var result = await _cachedCategoryService.GetCategoryByIdAsync(categoryId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(mockCategory.Id, result.Id);
        Assert.Equal(mockCategory.Name, result.Name);
        
        // Verify that the underlying service WAS called
        _mockCategoryService.Verify(s => s.GetCategoryByIdAsync(categoryId), Times.Once);
        
        // Verify that data was cached
        var cachedData = _memoryCache.Get($"category:{categoryId}");
        Assert.NotNull(cachedData);
    }
    public void Dispose()
    {
        _memoryCache.Dispose();
    }
}