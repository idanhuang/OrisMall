using Microsoft.Extensions.Caching.Memory;
using OrisMall.Core.DTOs;
using OrisMall.Core.Interfaces;

namespace OrisMall.Infrastructure.Services;

public class CachedCategoryService : ICategoryService
{
    private readonly CategoryService _categoryService; // Inject concrete service
    private readonly IMemoryCache _cache;
    private readonly TimeSpan _defaultCacheDuration = TimeSpan.FromHours(2); // Categories change rarely

    public CachedCategoryService(CategoryService categoryService, IMemoryCache cache)
    {
        _categoryService = categoryService;
        _cache = cache;
    }

    public async Task<IEnumerable<CategoryDto>> GetAllCategoriesAsync()
    {
        const string cacheKey = "categories:all";
        
        if (_cache.TryGetValue(cacheKey, out IEnumerable<CategoryDto>? cachedCategories))
        {
            Console.WriteLine("===========================================");
            Console.WriteLine($"CACHE HIT: {cacheKey}");
            Console.WriteLine("===========================================");
            return cachedCategories!;
        }

        Console.WriteLine("===========================================");
        Console.WriteLine($"CACHE MISS: {cacheKey} - Fetching from database");
        Console.WriteLine("===========================================");
        var categories = await _categoryService.GetAllCategoriesAsync();
        
        var cacheOptions = new MemoryCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = _defaultCacheDuration,
            SlidingExpiration = TimeSpan.FromMinutes(30), // Extend cache if accessed within 30 minutes
            Priority = CacheItemPriority.High, // Categories are important, don't evict easily
            Size = 1 // For size-based eviction
        };
        _cache.Set(cacheKey, categories, cacheOptions);
        return categories;
    }

    public async Task<CategoryDto?> GetCategoryByIdAsync(int id)
    {
        var cacheKey = $"category:{id}";

        if (_cache.TryGetValue(cacheKey, out CategoryDto? cachedCategory))
        {
            Console.WriteLine("===========================================");
            Console.WriteLine($"CACHE HIT: {cacheKey}");
            Console.WriteLine("===========================================");
            return cachedCategory;
        }

        Console.WriteLine("===========================================");
        Console.WriteLine($"CACHE MISS: {cacheKey} - Fetching from database");
        Console.WriteLine("===========================================");
        var category = await _categoryService.GetCategoryByIdAsync(id);
        
        if (category != null)
        {
            var cacheOptions = new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = _defaultCacheDuration,
                SlidingExpiration = TimeSpan.FromMinutes(30),
                Priority = CacheItemPriority.High,
                Size = 1
            };
            _cache.Set(cacheKey, category, cacheOptions);
        }
        return category;
    }

    public async Task<CategoryDto> CreateCategoryAsync(CreateCategoryDto createCategoryDto)
    {
        var category = await _categoryService.CreateCategoryAsync(createCategoryDto);
        
        // Invalidate cache when new category is created
        InvalidateCache();
        
        return category;
    }

    public async Task<CategoryDto> UpdateCategoryAsync(int id, UpdateCategoryDto updateCategoryDto)
    {
        var category = await _categoryService.UpdateCategoryAsync(id, updateCategoryDto);
        
        // Invalidate cache when category is updated
        InvalidateCache();
        
        return category;
    }

    public async Task DeleteCategoryAsync(int id)
    {
        await _categoryService.DeleteCategoryAsync(id);
        
        // Invalidate cache when category is deleted
        InvalidateCache();
    }

    private void InvalidateCache()
    {
        // Remove all categories from cache when data changes
        _cache.Remove("categories:all");
        
        // Note: Individual category cache entries will expire naturally
        // or we could implement a more sophisticated cache invalidation strategy
    }
}
