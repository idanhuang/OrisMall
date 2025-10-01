using OrisMall.Core.Entities;

namespace OrisMall.Core.Interfaces;

public interface ICartRepository
{
    Task<IEnumerable<CartItem>> GetByUserIdAsync(int userId);
    Task<CartItem?> GetByUserAndProductAsync(int userId, int productId);
    Task<CartItem> AddAsync(CartItem cartItem);
    Task<CartItem> UpdateAsync(CartItem cartItem);
    Task DeleteAsync(int id);
    Task DeleteByUserIdAsync(int userId);
    Task<bool> ExistsAsync(int id);
}