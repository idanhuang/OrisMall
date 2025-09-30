using System.ComponentModel.DataAnnotations;

namespace OrisMall.Core.Entities;

public class Product
{
    public int Id { get; set; }
    
    [Required]
    [MaxLength(200)]
    public string Name { get; set; } = string.Empty;
    
    [MaxLength(1000)]
    public string? Description { get; set; }
    
    [Required]
    [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than 0")]
    public decimal Price { get; set; }
    
    [Required]
    [Range(0, int.MaxValue, ErrorMessage = "Stock quantity cannot be negative")]
    public int StockQuantity { get; set; }
    
    [MaxLength(100)]
    public string? SKU { get; set; }
    
    [MaxLength(500)]
    public string? ImageUrl { get; set; }
    
    public bool IsActive { get; set; } = true;
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    public DateTime? UpdatedAt { get; set; }
    
    // Foreign Key
    public int CategoryId { get; set; }
    
    // Navigation Property
    public Category? Category { get; set; }
}
