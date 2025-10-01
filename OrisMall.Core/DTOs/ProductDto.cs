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
    public string? SKU { get; set; }
    
    [StringLength(500)]
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
    public string? SKU { get; set; }
    
    [StringLength(500)]
    public string? ImageUrl { get; set; }
    
    [Required]
    public int CategoryId { get; set; }
    
    public bool IsActive { get; set; }

}

public class ProductFilterDto
{
    public string? Name { get; set; }
    public int? CategoryId { get; set; }
    public decimal? MinPrice { get; set; }
    public decimal? MaxPrice { get; set; }
    public bool? InStock { get; set; }
    public string? SortBy { get; set; } // name | price | createdAt
    public string? SortDirection { get; set; } // asc | desc
    public int? Page { get; set; }
    public int? PageSize { get; set; }
}
