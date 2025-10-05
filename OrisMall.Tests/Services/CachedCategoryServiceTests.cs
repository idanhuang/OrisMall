using Microsoft.Extensions.Caching.Memory;
using Moq;
using OrisMall.Core.Interfaces;
using OrisMall.Infrastructure.Services;
using OrisMall.Tests.Helpers;

namespace OrisMall.Tests.Services;

public class CachedCategoryServiceTests : IDisposable
{
    private readonly Mock<ICategoryService> _mockCategoryService;
    private readonly Mock<ICategoryRepository> _mockRepository;
    private readonly Mock<IProductRepository> _mockProductRepository;
    private readonly IMemoryCache _memoryCache;
    private readonly CachedCategoryService _cachedCategoryService;
    private readonly CategoryService _realCategoryService;

    public CachedCategoryServiceTests()
    {
        _mockCategoryService = new Mock<ICategoryService>();
        _mockRepository = new Mock<ICategoryRepository>();
        _mockProductRepository = new Mock<IProductRepository>();
        _memoryCache = new MemoryCache(new MemoryCacheOptions());
        
        // Create a real CategoryService instance to pass to CachedCategoryService
        _realCategoryService = new CategoryService(_mockRepository.Object, _mockProductRepository.Object);
        _cachedCategoryService = new CachedCategoryService(_realCategoryService, _memoryCache);
    }

    public void Dispose()
    {
        _memoryCache.Dispose();
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
}