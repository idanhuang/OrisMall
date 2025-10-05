using Microsoft.Extensions.Caching.Memory;
using OrisMall.Core.DTOs;
using OrisMall.Core.Interfaces;

namespace OrisMall.Infrastructure.Services;

public class CachedCategoryService : ICategoryService
{
    private readonly CategoryService _categoryService;
    private readonly IMemoryCache _cache;
    private readonly TimeSpan _defaultCacheDuration = TimeSpan.FromHours(2);

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
            AbsoluteExpirationRelativeToNow = _defaultCacheDuration, // Hard deadline: cache expires after 2 hours regardless of access
            SlidingExpiration = TimeSpan.FromMinutes(30), // Cache expires after 30 minutes of no access
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
                AbsoluteExpirationRelativeToNow = _defaultCacheDuration, // cache expires after 2 hours regardless of access
                SlidingExpiration = TimeSpan.FromMinutes(30), // Cache expires after 30 minutes of no access
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
        
        InvalidateCache();
        
        return category;
    }

    public async Task DeleteCategoryAsync(int id)
    {
        await _categoryService.DeleteCategoryAsync(id);
        
        InvalidateCache();
    }

    private void InvalidateCache()
    {
        // Remove all categories from cache when data changes
        _cache.Remove("categories:all");
    }
}