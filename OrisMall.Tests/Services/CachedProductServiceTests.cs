using Microsoft.Extensions.Caching.Memory;
using Moq;
using OrisMall.Core.Interfaces;
using OrisMall.Infrastructure.Services;
using OrisMall.Tests.Helpers;

namespace OrisMall.Tests.Services;

public class CachedProductServiceTests : IDisposable
{
    private readonly Mock<IProductService> _mockProductService;
    private readonly IMemoryCache _memoryCache;
    private readonly CachedProductService _cachedProductService;

    public CachedProductServiceTests()
    {
        _mockProductService = new Mock<IProductService>();
        _memoryCache = new MemoryCache(new MemoryCacheOptions());
        _cachedProductService = new CachedProductService(_mockProductService.Object, _memoryCache);
    }

    public void Dispose()
    {
        _memoryCache.Dispose();
    }

    [Fact]
    public async Task GetAllProductsAsync_ShouldReturnCachedData_WhenCacheHit()
    {
        // Arrange
        var mockProducts = MockDataHelper.GetMockProductDtos();
        var cacheKey = "products:all";
        
        // Pre-populate cache
        _memoryCache.Set(cacheKey, mockProducts);

        // Act
        var result = await _cachedProductService.GetAllProductsAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(mockProducts.Count, result.Count());
        Assert.Equal(mockProducts.First().Id, result.First().Id);
        
        // Verify that the underlying service was NOT called
        _mockProductService.Verify(s => s.GetAllProductsAsync(), Times.Never);
    }
}