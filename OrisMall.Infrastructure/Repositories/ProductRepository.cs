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
            .Where(p => p.IsActive)
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
            .Where(p => p.CategoryId == categoryId && p.IsActive)
            .ToListAsync();
    }

    public async Task<IEnumerable<Product>> SearchAsync(string searchTerm)
    {
        return await _context.Products
            .Include(p => p.Category)
            .Where(p => p.IsActive && 
                       (p.Name.Contains(searchTerm) || 
                        p.Description!.Contains(searchTerm) ||
                        p.SKU!.Contains(searchTerm)))
            .ToListAsync();
    }

    public async Task<Product> AddAsync(Product product)
    {
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

    public async Task<bool> DeleteAsync(int id)
    {
        var product = await _context.Products.FindAsync(id);
        if (product == null)
            return false;

        _context.Products.Remove(product);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> ExistsAsync(int id)
    {
        return await _context.Products.AnyAsync(p => p.Id == id);
    }
}
