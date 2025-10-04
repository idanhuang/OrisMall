using System.ComponentModel.DataAnnotations;

namespace OrisMall.Core.DTOs;

public class ProductDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public decimal Price { get; set; }
    public int StockQuantity { get; set; }
    public string? SKU { get; set; }
    public string? ImageUrl { get; set; }
    public bool IsActive { get; set; }
    public int CategoryId { get; set; }
    public string CategoryName { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}

public class CreateProductDto
{
    [Required]
    [StringLength(200)]
    public string Name { get; set; } = string.Empty;
    
    [StringLength(1000)]
    public string? Description { get; set; }
    
    [Required]
    [Range(0.01, double.MaxValue)]
    public decimal Price { get; set; }
    
    [Range(0, int.MaxValue)]
    public int StockQuantity { get; set; }
    
    [StringLength(100)]
    [RegularExpression(@"^[A-Z0-9\-_]+$", ErrorMessage = "SKU can only contain uppercase letters, numbers, hyphens, and underscores. Valid example: ABC123")]
    public string? SKU { get; set; }
    
    [StringLength(500)]
    [RegularExpression(@"^https?://.+", ErrorMessage = "Image URL must be a valid HTTP/HTTPS URL. Valid example: https://example.com/image.jpg")]
    public string? ImageUrl { get; set; }
    
    [Required]
    public int CategoryId { get; set; }
}

public class UpdateProductDto
{
    [Required]
    [StringLength(200)]
    public string Name { get; set; } = string.Empty;
    
    [StringLength(1000)]
    public string? Description { get; set; }
    
    [Required]
    [Range(0.01, double.MaxValue)]
    public decimal Price { get; set; }
    
    [Range(0, int.MaxValue)]
    public int StockQuantity { get; set; }
    
    [StringLength(100)]
    [RegularExpression(@"^[A-Z0-9\-_]+$", ErrorMessage = "SKU can only contain uppercase letters, numbers, hyphens, and underscores. Valid example: ABC123")]
    public string? SKU { get; set; }
    
    [StringLength(500)]
    [RegularExpression(@"^https?://.+", ErrorMessage = "Image URL must be a valid HTTP/HTTPS URL. Valid example: https://example.com/image.jpg")]
    public string? ImageUrl { get; set; }
    
    [Required]
    public int CategoryId { get; set; }
    
    public bool IsActive { get; set; }

}

public class ProductFilterDto
{
    [StringLength(100)]
    public string? Name { get; set; }
    
    [Range(1, int.MaxValue, ErrorMessage = "Category ID must be a positive number")]
    public int? CategoryId { get; set; }
    
    [Range(0.01, double.MaxValue, ErrorMessage = "Minimum price must be greater than 0")]
    public decimal? MinPrice { get; set; }
    
    [Range(0.01, double.MaxValue, ErrorMessage = "Maximum price must be greater than 0")]
    public decimal? MaxPrice { get; set; }
    
    public bool? InStock { get; set; }
    
    [RegularExpression(@"^(name|price|createdAt)$", ErrorMessage = "SortBy must be 'name', 'price', or 'createdAt'. Valid example: name")]
    public string? SortBy { get; set; } // name | price | createdAt
    
    [RegularExpression(@"^(asc|desc)$", ErrorMessage = "SortDirection must be 'asc' or 'desc'. Valid example: asc")]
    public string? SortDirection { get; set; } // asc | desc
    
    [Range(1, 1000, ErrorMessage = "Page must be between 1 and 1000")]
    public int? Page { get; set; }
    
    [Range(1, 100, ErrorMessage = "PageSize must be between 1 and 100")]
    public int? PageSize { get; set; }
}
