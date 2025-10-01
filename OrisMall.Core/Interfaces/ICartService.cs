using OrisMall.Core.DTOs;

namespace OrisMall.Core.Interfaces;

public interface ICartService
{
    Task<CartDto> GetCartAsync(string sessionId);
    Task<CartDto> AddToCartAsync(string sessionId, AddToCartDto addToCartDto);
    Task<CartDto> UpdateCartItemAsync(string sessionId, UpdateCartItemDto updateCartItemDto);
    Task<CartDto> RemoveFromCartAsync(string sessionId, int productId);
    Task<CartDto> ClearCartAsync(string sessionId);
    Task<bool> CartExistsAsync(string sessionId);
    Task<int> GetCartItemCountAsync(string sessionId);
}