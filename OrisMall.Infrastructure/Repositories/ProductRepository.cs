using Microsoft.EntityFrameworkCore;
using OrisMall.Core.Entities;
using OrisMall.Core.Interfaces;
using OrisMall.Infrastructure.Data;

namespace OrisMall.Infrastructure.Repositories;

public class ProductRepository : IProductRepository
{
    private readonly OrisMallDbContext _context;

    public ProductRepository(OrisMallDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Product>> GetAllAsync()
    {
        return await _context.Products
            .Include(p => p.Category)
            .ToListAsync();
    }

    public async Task<Product?> GetByIdAsync(int id)
    {
        return await _context.Products
            .Include(p => p.Category)
            .FirstOrDefaultAsync(p => p.Id == id);
    }

    public async Task<IEnumerable<Product>> GetByCategoryIdAsync(int categoryId)
    {
        return await _context.Products
            .Include(p => p.Category)
            .Where(p => p.CategoryId == categoryId)
            .ToListAsync();
    }

    public async Task<IEnumerable<Product>> SearchAsync(string searchTerm)
    {
        return await _context.Products
            .Include(p => p.Category)
            .Where(p => p.Name.Contains(searchTerm) ||
                       p.Description!.Contains(searchTerm) ||
                       p.SKU!.Contains(searchTerm))
            .ToListAsync();
    }

    public async Task<(IEnumerable<Product> Items, int TotalCount)> FilterAsync(
        string? name,
        int? categoryId,
        decimal? minPrice,
        decimal? maxPrice,
        bool? inStock,
        string? sortBy,
        string? sortDirection,
        int? page,
        int? pageSize)
    {
        IQueryable<Product> query = _context.Products
            .Include(p => p.Category);

        if (!string.IsNullOrWhiteSpace(name))
        {
            var term = name.Trim();
            query = query.Where(p => p.Name.Contains(term) || (p.Description != null && p.Description.Contains(term)));
        }

        if (categoryId.HasValue)
        {
            query = query.Where(p => p.CategoryId == categoryId.Value);
        }

        if (minPrice.HasValue)
        {
            query = query.Where(p => p.Price >= minPrice.Value);
        }

        if (maxPrice.HasValue)
        {
            query = query.Where(p => p.Price <= maxPrice.Value);
        }

        if (inStock.HasValue)
        {
            query = inStock.Value
                ? query.Where(p => p.StockQuantity > 0)
                : query.Where(p => p.StockQuantity == 0);
        }

        // Sorting
        bool desc = string.Equals(sortDirection, "desc", StringComparison.OrdinalIgnoreCase);
        switch (sortBy?.ToLowerInvariant())
        {
            case "price":
                query = desc ? query.OrderByDescending(p => p.Price) : query.OrderBy(p => p.Price);
                break;
            case "name":
                query = desc ? query.OrderByDescending(p => p.Name) : query.OrderBy(p => p.Name);
                break;
            case "createdat":
                query = desc ? query.OrderByDescending(p => p.CreatedAt) : query.OrderBy(p => p.CreatedAt);
                break;
            default:
                query = query.OrderBy(p => p.Name);
                break;
        }

        var total = await query.CountAsync();

        if (page.HasValue && pageSize.HasValue && page.Value > 0 && pageSize.Value > 0)
        {
            // Pagination Formula Explanation:
            // skip = (page - 1) * pageSize
            // 
            // Why (page - 1)?
            // - Page 1 should start at item 0 (no items to skip)
            // - Page 2 should start at item pageSize (skip 1 page worth of items)
            // - Page 3 should start at item 2*pageSize (skip 2 pages worth of items)
            // - So we need (page - 1) to convert 1-based page numbers to 0-based skip counts

            int skip = (page.Value - 1) * pageSize.Value;
            query = query.Skip(skip).Take(pageSize.Value);
        }

        var items = await query.ToListAsync();
        return (items, total);
    }

    public async Task<Product> AddAsync(Product product)
    {
        product.CreatedAt = DateTime.UtcNow;
        _context.Products.Add(product);
        await _context.SaveChangesAsync();
        return product;
    }

    public async Task<Product> UpdateAsync(Product product)
    {
        product.UpdatedAt = DateTime.UtcNow;
        _context.Products.Update(product);
        await _context.SaveChangesAsync();
        return product;
    }

    public async Task DeleteAsync(int id)
    {
        var product = await _context.Products.FindAsync(id);
        if (product != null)
        {
            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<bool> ExistsAsync(int id)
    {
        return await _context.Products.AnyAsync(p => p.Id == id);
    }

    public async Task<bool> HasProductsInCategoryAsync(int categoryId)
    {
        return await _context.Products.AnyAsync(p => p.CategoryId == categoryId);
    }
}

