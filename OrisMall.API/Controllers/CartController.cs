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

    [HttpGet]
    public async Task<ActionResult<CartDto>> GetCart()
    {
        var sessionId = GetOrCreateSessionId();
        _logger.LogInformation("Retrieving cart for session {SessionId}", sessionId);
        
        var cart = await _cartService.GetCartAsync(sessionId);
        
        _logger.LogInformation("Cart retrieved successfully for session {SessionId}", sessionId);
        return Ok(cart);
    }

    [HttpPost("add")]
    public async Task<ActionResult<CartDto>> AddToCart(AddToCartDto addToCartDto)
    {
        if (!ModelState.IsValid)
        {
            _logger.LogWarning("Invalid add to cart request model state");
            return BadRequest(ModelState);
        }

        var sessionId = GetOrCreateSessionId();
        _logger.LogInformation("Adding product {ProductId} to cart for session {SessionId}", addToCartDto.ProductId, sessionId);
        
        var cart = await _cartService.AddToCartAsync(sessionId, addToCartDto);
        
        _logger.LogInformation("Product {ProductId} added to cart successfully for session {SessionId}", addToCartDto.ProductId, sessionId);
        return CreatedAtAction(nameof(GetCart), null, cart);
    }

    [HttpPut("update")]
    public async Task<IActionResult> UpdateCartItem(UpdateCartItemDto updateCartItemDto)
    {
        if (!ModelState.IsValid)
        {
            _logger.LogWarning("Invalid update cart item request model state");
            return BadRequest(ModelState);
        }

        var sessionId = GetOrCreateSessionId();
        _logger.LogInformation("Updating cart item {ProductId} for session {SessionId}", updateCartItemDto.ProductId, sessionId);
        
        await _cartService.UpdateCartItemAsync(sessionId, updateCartItemDto);
        
        _logger.LogInformation("Cart item {ProductId} updated successfully for session {SessionId}", updateCartItemDto.ProductId, sessionId);
        return NoContent();
    }

    [HttpDelete("remove/{productId}")]
    public async Task<IActionResult> RemoveFromCart(int productId)
    {
        var sessionId = GetOrCreateSessionId();
        await _cartService.RemoveFromCartAsync(sessionId, productId);
        return NoContent();
    }

    [HttpDelete("clear")]
    public async Task<IActionResult> ClearCart()
    {
        var sessionId = GetOrCreateSessionId();
        _logger.LogInformation("Clearing cart for session {SessionId}", sessionId);
        
        await _cartService.ClearCartAsync(sessionId);
        
        _logger.LogInformation("Cart cleared successfully for session {SessionId}", sessionId);
        return NoContent();
    }

    [HttpGet("count")]
    public async Task<ActionResult<int>> GetCartItemCount()
    {
        var sessionId = GetOrCreateSessionId();
        _logger.LogInformation("Getting cart item count for session {SessionId}", sessionId);
        
        var count = await _cartService.GetCartItemCountAsync(sessionId);
        
        _logger.LogInformation("Cart item count retrieved for session {SessionId}: {Count}", sessionId, count);
        return Ok(count);
    }

    [HttpGet("exists")]
    public async Task<ActionResult<bool>> CartExists()
    {
        var sessionId = GetOrCreateSessionId();
        _logger.LogInformation("Checking if cart exists for session {SessionId}", sessionId);
        
        var exists = await _cartService.CartExistsAsync(sessionId);
        
        _logger.LogInformation("Cart existence check completed for session {SessionId}: {Exists}", sessionId, exists);
        return Ok(exists);
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