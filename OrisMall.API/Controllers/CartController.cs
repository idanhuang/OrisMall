using Microsoft.AspNetCore.Mvc;
using OrisMall.Core.DTOs;
using OrisMall.Core.Interfaces;

namespace OrisMall.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CartController : ControllerBase
{
    private readonly ICartService _cartService;

    public CartController(ICartService cartService)
    {
        _cartService = cartService;
    }

    [HttpGet]
    public async Task<ActionResult<CartDto>> GetCart()
    {
        var sessionId = GetOrCreateSessionId();
        var cart = await _cartService.GetCartAsync(sessionId);
        return Ok(cart);
    }

    [HttpPost("add")]
    public async Task<ActionResult<CartDto>> AddToCart(AddToCartDto addToCartDto)
    {
        try
        {
            var sessionId = GetOrCreateSessionId();
            var cart = await _cartService.AddToCartAsync(sessionId, addToCartDto);
            return CreatedAtAction(nameof(GetCart), null, cart);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPut("update")]
    public async Task<IActionResult> UpdateCartItem(UpdateCartItemDto updateCartItemDto)
    {
        try
        {
            var sessionId = GetOrCreateSessionId();
            await _cartService.UpdateCartItemAsync(sessionId, updateCartItemDto);
            return NoContent();
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpDelete("remove/{productId}")]
    public async Task<IActionResult> RemoveFromCart(int productId)
    {
        try
        {
            var sessionId = GetOrCreateSessionId();
            await _cartService.RemoveFromCartAsync(sessionId, productId);
            return NoContent();
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpDelete("clear")]
    public async Task<IActionResult> ClearCart()
    {
        var sessionId = GetOrCreateSessionId();
        await _cartService.ClearCartAsync(sessionId);
        return NoContent();
    }

    [HttpGet("count")]
    public async Task<ActionResult<int>> GetCartItemCount()
    {
        var sessionId = GetOrCreateSessionId();
        var count = await _cartService.GetCartItemCountAsync(sessionId);
        return Ok(count);
    }

    [HttpGet("exists")]
    public async Task<ActionResult<bool>> CartExists()
    {
        var sessionId = GetOrCreateSessionId();
        var exists = await _cartService.CartExistsAsync(sessionId);
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