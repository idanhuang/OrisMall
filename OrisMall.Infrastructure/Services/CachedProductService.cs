using Microsoft.Extensions.Caching.Memory;
using OrisMall.Core.DTOs;
using OrisMall.Core.Interfaces;

namespace OrisMall.Infrastructure.Services;

public class CachedProductService : IProductService
{
    private readonly IProductService _productService;
    private readonly IMemoryCache _cache;
    private readonly TimeSpan _productCacheDuration = TimeSpan.FromMinutes(30); // Products change more frequently
    private readonly TimeSpan _searchCacheDuration = TimeSpan.FromMinutes(15); // Search results cache shorter

    public CachedProductService(IProductService productService, IMemoryCache cache)
    {
        _productService = productService;
        _cache = cache;
    }

    public async Task<IEnumerable<ProductDto>> GetAllProductsAsync()
    {
        const string cacheKey = "products:all";
        
        if (_cache.TryGetValue(cacheKey, out IEnumerable<ProductDto>? cachedProducts))
        {
            Console.WriteLine("===========================================");
            Console.WriteLine($"CACHE HIT: {cacheKey}");
            Console.WriteLine("===========================================");
            return cachedProducts!;
        }

        Console.WriteLine("===========================================");
        Console.WriteLine($"CACHE MISS: {cacheKey} - Fetching from database");
        Console.WriteLine("===========================================");
        var products = await _productService.GetAllProductsAsync();
        
        var cacheOptions = new MemoryCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = _productCacheDuration,
            SlidingExpiration = TimeSpan.FromMinutes(10),
            Priority = CacheItemPriority.High,
            Size = 5 // Products list is larger, assign higher size
        };
        _cache.Set(cacheKey, products, cacheOptions);
        return products;
    }

    public async Task<ProductDto?> GetProductByIdAsync(int id)
    {
        var cacheKey = $"product:{id}";

        if (_cache.TryGetValue(cacheKey, out ProductDto? cachedProduct))
        {
            Console.WriteLine("===========================================");
            Console.WriteLine($"CACHE HIT: {cacheKey}");
            Console.WriteLine("===========================================");
            return cachedProduct;
        }

        Console.WriteLine("===========================================");
        Console.WriteLine($"CACHE MISS: {cacheKey} - Fetching from database");
        Console.WriteLine("===========================================");
        var product = await _productService.GetProductByIdAsync(id);
        
        if (product != null)
        {
            var cacheOptions = new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = _productCacheDuration,
                SlidingExpiration = TimeSpan.FromMinutes(10),
                Priority = CacheItemPriority.Normal,
                Size = 1
            };
            _cache.Set(cacheKey, product, cacheOptions);
        }
        return product;
    }

    public async Task<IEnumerable<ProductDto>> GetProductsByCategoryAsync(int categoryId)
    {
        string cacheKey = $"products:category:{categoryId}";
        
        if (_cache.TryGetValue(cacheKey, out IEnumerable<ProductDto>? cachedProducts))
        {
            return cachedProducts!;
        }

        var products = await _productService.GetProductsByCategoryAsync(categoryId);
        
        var cacheOptions = new MemoryCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = _productCacheDuration,
            SlidingExpiration = TimeSpan.FromMinutes(10),
            Priority = CacheItemPriority.Normal,
            Size = 3
        };
        _cache.Set(cacheKey, products, cacheOptions);
        return products;
    }

    public async Task<IEnumerable<ProductDto>> SearchProductsAsync(string searchTerm)
    {
        // Normalize search term for consistent caching
        string normalizedSearchTerm = searchTerm.Trim().ToLowerInvariant();
        string cacheKey = $"search:{normalizedSearchTerm}";
        
        if (_cache.TryGetValue(cacheKey, out IEnumerable<ProductDto>? cachedResults))
        {
            return cachedResults!;
        }

        var products = await _productService.SearchProductsAsync(searchTerm);
        
        var cacheOptions = new MemoryCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = _searchCacheDuration, // Shorter cache for search
            Priority = CacheItemPriority.Low, // Search results can be evicted more easily
            Size = 2
        };
        _cache.Set(cacheKey, products, cacheOptions);
        return products;
    }

    public async Task<(IEnumerable<ProductDto> Items, int TotalCount)> FilterProductsAsync(ProductFilterDto filter)
    {
        // Create a cache key based on filter parameters
        string cacheKey = $"filter:{filter.Name}:{filter.CategoryId}:{filter.MinPrice}:{filter.MaxPrice}:{filter.InStock}:{filter.SortBy}:{filter.SortDirection}:{filter.Page}:{filter.PageSize}";
        
        if (_cache.TryGetValue(cacheKey, out (IEnumerable<ProductDto> Items, int TotalCount) cachedResult))
        {
            return cachedResult;
        }

        var result = await _productService.FilterProductsAsync(filter);
        
        var cacheOptions = new MemoryCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = _searchCacheDuration, // Shorter cache for filters
            Priority = CacheItemPriority.Low,
            Size = 2
        };
        _cache.Set(cacheKey, result, cacheOptions);
        return result;
    }

    public async Task<ProductDto> CreateProductAsync(CreateProductDto createProductDto)
    {
        var product = await _productService.CreateProductAsync(createProductDto);
        
        // Invalidate relevant caches when new product is created
        InvalidateProductCaches();
        
        return product;
    }

    public async Task<ProductDto> UpdateProductAsync(int id, UpdateProductDto updateProductDto)
    {
        var product = await _productService.UpdateProductAsync(id, updateProductDto);
        
        // Invalidate relevant caches when product is updated
        InvalidateProductCaches();
        InvalidateSpecificProductCache(id);
        
        return product;
    }

    public async Task DeleteProductAsync(int id)
    {
        await _productService.DeleteProductAsync(id);
        
        // Invalidate relevant caches when product is deleted
        InvalidateProductCaches();
        InvalidateSpecificProductCache(id);
    }

    private void InvalidateProductCaches()
    {
        // Remove product list caches
        _cache.Remove("products:all");
        
        // Note: Category-specific and search caches will expire naturally
        // In a more sophisticated implementation, we could track and invalidate specific caches
    }

    private void InvalidateSpecificProductCache(int productId)
    {
        _cache.Remove($"product:{productId}");
    }
}
