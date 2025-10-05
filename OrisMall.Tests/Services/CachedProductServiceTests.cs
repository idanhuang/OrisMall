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

    [Fact]
    public async Task GetAllProductsAsync_ShouldCallUnderlyingService_WhenCacheMiss()
    {
        // Arrange
        var mockProducts = MockDataHelper.GetMockProductDtos();
        _mockProductService.Setup(s => s.GetAllProductsAsync()).ReturnsAsync(mockProducts);

        // Act
        var result = await _cachedProductService.GetAllProductsAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(mockProducts.Count, result.Count());
        Assert.Equal(mockProducts.First().Id, result.First().Id);
        
        // Verify that the underlying service WAS called
        _mockProductService.Verify(s => s.GetAllProductsAsync(), Times.Once);
        
        // Verify that data was cached
        var cachedData = _memoryCache.Get("products:all");
        Assert.NotNull(cachedData);
    }

    [Fact]
    public async Task GetProductByIdAsync_ShouldCallUnderlyingService_WhenCacheMiss()
    {
        // Arrange
        var productId = 1;
        var mockProduct = MockDataHelper.GetMockProductDtos().First();
        _mockProductService.Setup(s => s.GetProductByIdAsync(productId)).ReturnsAsync(mockProduct);

        // Act
        var result = await _cachedProductService.GetProductByIdAsync(productId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(mockProduct.Id, result.Id);
        Assert.Equal(mockProduct.Name, result.Name);
        
        // Verify that the underlying service WAS called
        _mockProductService.Verify(s => s.GetProductByIdAsync(productId), Times.Once);
        
        // Verify that data was cached
        var cachedData = _memoryCache.Get($"product:{productId}");
        Assert.NotNull(cachedData);
    }

    [Fact]
    public async Task GetProductsByCategoryAsync_ShouldCallUnderlyingService_WhenCacheMiss()
    {
        // Arrange
        var categoryId = 1;
        var mockProducts = MockDataHelper.GetMockProductDtos().Where(p => p.CategoryId == categoryId);
        _mockProductService.Setup(s => s.GetProductsByCategoryAsync(categoryId)).ReturnsAsync(mockProducts);

        // Act
        var result = await _cachedProductService.GetProductsByCategoryAsync(categoryId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(mockProducts.Count(), result.Count());
        
        // Verify that the underlying service WAS called
        _mockProductService.Verify(s => s.GetProductsByCategoryAsync(categoryId), Times.Once);
        
        // Verify that data was cached
        var cachedData = _memoryCache.Get($"products:category:{categoryId}");
        Assert.NotNull(cachedData);
    }

    public void Dispose()
    {
        _memoryCache.Dispose();
    }
}