using Microsoft.AspNetCore.Mvc;
using OrisMall.Core.DTOs;
using OrisMall.Core.Interfaces;

namespace OrisMall.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CartController : ControllerBase
{
    private readonly ICartService _cartService;
    private readonly ILogger<CartController> _logger;

    public CartController(ICartService cartService, ILogger<CartController> logger)
    {
        _cartService = cartService;
        _logger = logger;
    }

    /// <summary>
    /// Get current cart contents
    /// </summary>
    /// <returns>Cart information with items</returns>
    [HttpGet]
    public async Task<ActionResult<CartDto>> GetCart()
    {
        var sessionId = GetOrCreateSessionId();
        _logger.LogInformation("Retrieving cart for session {SessionId}", sessionId);
        
        var cart = await _cartService.GetCartAsync(sessionId);
        
        _logger.LogInformation("Cart retrieved successfully for session {SessionId}", sessionId);
        return Ok(cart);
    }

    /// <summary>
    /// Add product to cart
    /// </summary>
    /// <returns>Updated cart information</returns>
    [HttpPost("add")]
    public async Task<ActionResult<CartDto>> AddToCart(AddToCartDto addToCartDto)
    {
        var sessionId = GetOrCreateSessionId();
        _logger.LogInformation("Adding product {ProductId} to cart for session {SessionId}", addToCartDto.ProductId, sessionId);
        
        var cart = await _cartService.AddToCartAsync(sessionId, addToCartDto);
        
        _logger.LogInformation("Product {ProductId} added to cart successfully for session {SessionId}", addToCartDto.ProductId, sessionId);
        return CreatedAtAction(nameof(GetCart), new { }, cart);
    }

    /// <summary>
    /// Update cart item quantity
    /// </summary>
    /// <returns>No content on success</returns>
    [HttpPut("update")]
    public async Task<IActionResult> UpdateCartItem(UpdateCartItemDto updateCartItemDto)
    {
        var sessionId = GetOrCreateSessionId();
        _logger.LogInformation("Updating cart item {ProductId} for session {SessionId}", updateCartItemDto.ProductId, sessionId);
        
        await _cartService.UpdateCartItemAsync(sessionId, updateCartItemDto);
        
        _logger.LogInformation("Cart item {ProductId} updated successfully for session {SessionId}", updateCartItemDto.ProductId, sessionId);
        return NoContent();
    }

    /// <summary>
    /// Remove product from cart
    /// </summary>
    /// <returns>No content on success</returns>
    [HttpDelete("remove/{productId}")]
    public async Task<IActionResult> RemoveFromCart(int productId)
    {
        var sessionId = GetOrCreateSessionId();
        _logger.LogInformation("Removing product {ProductId} from cart for session {SessionId}", productId, sessionId);
        
        await _cartService.RemoveFromCartAsync(sessionId, productId);
        
        _logger.LogInformation("Product {ProductId} removed from cart successfully for session {SessionId}", productId, sessionId);
        return NoContent();
    }

    /// <summary>
    /// Clear all items from cart
    /// </summary>
    /// <returns>No content on success</returns>
    [HttpDelete("clear")]
    public async Task<IActionResult> ClearCart()
    {
        var sessionId = GetOrCreateSessionId();
        _logger.LogInformation("Clearing cart for session {SessionId}", sessionId);
        
        await _cartService.ClearCartAsync(sessionId);
        
        _logger.LogInformation("Cart cleared successfully for session {SessionId}", sessionId);
        return NoContent();
    }

    /// <summary>
    /// Get total number of items in cart
    /// </summary>
    /// <returns>Cart item count</returns>
    [HttpGet("count")]
    public async Task<ActionResult<int>> GetCartItemCount()
    {
        var sessionId = GetOrCreateSessionId();
        _logger.LogInformation("Getting cart item count for session {SessionId}", sessionId);
        
        var count = await _cartService.GetCartItemCountAsync(sessionId);
        
        _logger.LogInformation("Cart item count retrieved for session {SessionId}: {Count}", sessionId, count);
        return Ok(count);
    }

    private string GetOrCreateSessionId()
    {
        var sessionId = HttpContext.Session.GetString("SessionId");
        if (string.IsNullOrEmpty(sessionId))
        {
            sessionId = Guid.NewGuid().ToString();
            HttpContext.Session.SetString("SessionId", sessionId);
        }
        return sessionId;
    }
}