using OrisMall.Core.DTOs;
using OrisMall.Core.Interfaces;
using OrisMall.Core.Exceptions;
using Microsoft.AspNetCore.Http;
using System.Text.Json;

namespace OrisMall.Infrastructure.Services;

public class CartService : ICartService
{
    private readonly IProductService _productService;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private const string CartSessionKey = "ShoppingCart";
    
    // In-memory storage for demo purposes (replace with Redis or database in production)
    private static readonly Dictionary<string, string> _cartStorage = new();

    public CartService(IProductService productService, IHttpContextAccessor httpContextAccessor)
    {
        _productService = productService;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<CartDto> GetCartAsync(string sessionId)
    {
        var cartKey = $"{CartSessionKey}_{sessionId}";
        
        if (!_cartStorage.TryGetValue(cartKey, out var cartJson) || string.IsNullOrEmpty(cartJson))
            return CreateEmptyCart(sessionId);

        try
        {
            var cart = JsonSerializer.Deserialize<CartDto>(cartJson);
            if (cart == null)
                return CreateEmptyCart(sessionId);

            // Update cart totals
            await UpdateCartTotalsAsync(cart);
            return cart;
        }
        catch
        {
            return CreateEmptyCart(sessionId);
        }
    }

    public async Task<CartDto> AddToCartAsync(string sessionId, AddToCartDto addToCartDto)
    {
        var cart = await GetCartAsync(sessionId);
        
        // Get product details
        var product = await _productService.GetProductByIdAsync(addToCartDto.ProductId);
        if (product == null)
            throw new ArgumentException("Product not found");

        // Check if product is already in cart
        var existingItem = cart.Items.FirstOrDefault(x => x.ProductId == addToCartDto.ProductId);
        
        if (existingItem != null)
        {
            // Update existing item
            existingItem.Quantity += addToCartDto.Quantity;
            existingItem.TotalPrice = existingItem.UnitPrice * existingItem.Quantity;
        }
        else
        {
            // Add new item
            var newItem = new CartItemDto
            {
                ProductId = product.Id,
                ProductName = product.Name,
                ProductDescription = product.Description ?? string.Empty,
                UnitPrice = product.Price,
                Quantity = addToCartDto.Quantity,
                TotalPrice = product.Price * addToCartDto.Quantity,
                ProductImageUrl = product.ImageUrl,
                AddedAt = DateTime.UtcNow
            };
            cart.Items.Add(newItem);
        }

        // Update cart totals and save
        await UpdateCartTotalsAsync(cart);
        await SaveCartAsync(sessionId, cart);
        
        return cart;
    }

    public async Task<CartDto> UpdateCartItemAsync(string sessionId, UpdateCartItemDto updateCartItemDto)
    {
        var cart = await GetCartAsync(sessionId);
        var item = cart.Items.FirstOrDefault(x => x.ProductId == updateCartItemDto.ProductId);
        
        if (item == null)
            throw new ArgumentException("Item not found in cart");

        if (updateCartItemDto.Quantity == 0)
        {
            // Remove item if quantity is 0
            cart.Items.Remove(item);
        }
        else
        {
            // Update quantity
            item.Quantity = updateCartItemDto.Quantity;
            item.TotalPrice = item.UnitPrice * item.Quantity;
        }

        // Update cart totals and save
        await UpdateCartTotalsAsync(cart);
        await SaveCartAsync(sessionId, cart);
        
        return cart;
    }

    public async Task<CartDto> RemoveFromCartAsync(string sessionId, int productId)
    {
        var cart = await GetCartAsync(sessionId);
        var item = cart.Items.FirstOrDefault(x => x.ProductId == productId);
        
        if (item == null)
            throw new NotFoundException("Product", productId);
        
        cart.Items.Remove(item);
        await UpdateCartTotalsAsync(cart);
        await SaveCartAsync(sessionId, cart);
        
        return cart;
    }

    public async Task<CartDto> ClearCartAsync(string sessionId)
    {
        var cart = CreateEmptyCart(sessionId);
        await SaveCartAsync(sessionId, cart);
        return cart;
    }

    public async Task<int> GetCartItemCountAsync(string sessionId)
    {
        var cart = await GetCartAsync(sessionId);
        return cart.TotalItems;
    }

    private Task UpdateCartTotalsAsync(CartDto cart)
    {
        cart.TotalItems = cart.Items.Sum(x => x.Quantity);
        cart.TotalAmount = cart.Items.Sum(x => x.TotalPrice);
        cart.UpdatedAt = DateTime.UtcNow;
        return Task.CompletedTask;
    }

    private async Task SaveCartAsync(string sessionId, CartDto cart)
    {
        var cartKey = $"{CartSessionKey}_{sessionId}";
        var cartJson = JsonSerializer.Serialize(cart);
        _cartStorage[cartKey] = cartJson;
        await Task.CompletedTask;
    }

    private CartDto CreateEmptyCart(string sessionId)
    {
        return new CartDto
        {
            SessionId = sessionId,
            Items = new List<CartItemDto>(),
            TotalAmount = 0,
            TotalItems = 0,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
    }
}
