using System.ComponentModel.DataAnnotations;

namespace OrisMall.Core.DTOs;

public class CartDto
{
    public string SessionId { get; set; } = string.Empty;
    public List<CartItemDto> Items { get; set; } = new List<CartItemDto>();
    public decimal TotalAmount { get; set; }
    public int TotalItems { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

public class CartItemDto
{
    public int ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public string ProductDescription { get; set; } = string.Empty;
    public decimal UnitPrice { get; set; }
    public int Quantity { get; set; }
    public decimal TotalPrice { get; set; }
    public string? ProductImageUrl { get; set; }
    public DateTime AddedAt { get; set; }
}

public class AddToCartDto
{
    [Required]
    public int ProductId { get; set; }
    
    [Required]
    [Range(1, int.MaxValue, ErrorMessage = "Quantity must be at least 1")]
    public int Quantity { get; set; } = 1;
}

public class UpdateCartItemDto
{
    [Required]
    public int ProductId { get; set; }
    
    [Required]
    [Range(0, int.MaxValue, ErrorMessage = "Quantity must be 0 or greater")]
    public int Quantity { get; set; }
}

public class RemoveFromCartDto
{
    [Required]
    public int ProductId { get; set; }
}