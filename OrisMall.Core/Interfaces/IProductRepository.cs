using OrisMall.Core.Entities;

namespace OrisMall.Core.Interfaces;

public interface IProductRepository
{
    Task<IEnumerable<Product>> GetAllAsync();
    Task<Product?> GetByIdAsync(int id);
    Task<IEnumerable<Product>> GetByCategoryIdAsync(int categoryId);
    Task<IEnumerable<Product>> SearchAsync(string searchTerm);
    Task<Product> AddAsync(Product product);
    Task<Product> UpdateAsync(Product product);
    Task<bool> DeleteAsync(int id);
    Task<bool> ExistsAsync(int id);
}
