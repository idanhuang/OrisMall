using Microsoft.EntityFrameworkCore;
using OrisMall.Core.Entities;
using OrisMall.Core.Interfaces;
using OrisMall.Infrastructure.Data;

namespace OrisMall.Infrastructure.Repositories;

public class CartRepository : ICartRepository
{
    private readonly OrisMallDbContext _context;

    public CartRepository(OrisMallDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<CartItem>> GetByUserIdAsync(int userId)
    {
        return await _context.CartItems
            .Include(ci => ci.Product)
            .ThenInclude(p => p.Category)
            .Where(ci => ci.UserId == userId)
            .OrderBy(ci => ci.CreatedAt)
            .ToListAsync();
    }

    public async Task<CartItem?> GetByUserAndProductAsync(int userId, int productId)
    {
        return await _context.CartItems
            .Include(ci => ci.Product)
            .FirstOrDefaultAsync(ci => ci.UserId == userId && ci.ProductId == productId);
    }

    public async Task<CartItem> AddAsync(CartItem cartItem)
    {
        cartItem.CreatedAt = DateTime.UtcNow;
        _context.CartItems.Add(cartItem);
        await _context.SaveChangesAsync();
        return cartItem;
    }

    public async Task<CartItem> UpdateAsync(CartItem cartItem)
    {
        cartItem.UpdatedAt = DateTime.UtcNow;
        _context.CartItems.Update(cartItem);
        await _context.SaveChangesAsync();
        return cartItem;
    }

    public async Task DeleteAsync(int id)
    {
        var cartItem = await _context.CartItems.FindAsync(id);
        if (cartItem != null)
        {
            _context.CartItems.Remove(cartItem);
            await _context.SaveChangesAsync();
        }
    }

    public async Task DeleteByUserIdAsync(int userId)
    {
        var cartItems = await _context.CartItems
            .Where(ci => ci.UserId == userId)
            .ToListAsync();
            
        if (cartItems.Any())
        {
            _context.CartItems.RemoveRange(cartItems);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<bool> ExistsAsync(int id)
    {
        return await _context.CartItems.AnyAsync(ci => ci.Id == id);
    }
}




