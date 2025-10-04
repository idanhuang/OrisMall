using System.ComponentModel.DataAnnotations;

namespace OrisMall.Core.DTOs;

public class CategoryDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? ImageUrl { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class CreateCategoryDto
{
    [Required]
    [StringLength(20, MinimumLength = 2)]
    [RegularExpression(@"^[a-zA-Z0-9\s\-_\.]+$", ErrorMessage = "Category name can only contain letters, numbers, spaces, hyphens, underscores, and dots. Valid example: Electronics")]
    public string Name { get; set; } = string.Empty;
    
    [StringLength(50)]
    public string? Description { get; set; }
    
    [StringLength(200)]
    [RegularExpression(@"^https?://.+", ErrorMessage = "Image URL must be a valid HTTP/HTTPS URL. Valid example: https://example.com/category.jpg")]
    public string? ImageUrl { get; set; }
}

public class UpdateCategoryDto
{
    [Required]
    [StringLength(100, MinimumLength = 2)]
    [RegularExpression(@"^[a-zA-Z0-9\s\-_\.]+$", ErrorMessage = "Category name can only contain letters, numbers, spaces, hyphens, underscores, and dots. Valid example: Electronics")]
    public string Name { get; set; } = string.Empty;
    
    [StringLength(500)]
    public string? Description { get; set; }
    
    [StringLength(200)]
    [RegularExpression(@"^https?://.+", ErrorMessage = "Image URL must be a valid HTTP/HTTPS URL. Valid example: https://example.com/category.jpg")]
    public string? ImageUrl { get; set; }
    
    public bool IsActive { get; set; }
}




