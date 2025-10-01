using System.ComponentModel.DataAnnotations;

namespace OrisMall.Core.Entities;

public class OrderItem
{
    public int Id { get; set; }
    
    public int OrderId { get; set; }
    
    public int ProductId { get; set; }
    
    [Required]
    [StringLength(200)]
    public string ProductName { get; set; } = string.Empty;
    
    [Range(1, int.MaxValue)]
    public int Quantity { get; set; }
    
    [Range(0.01, double.MaxValue)]
    public decimal UnitPrice { get; set; }
    
    [Range(0, double.MaxValue)]
    public decimal TotalPrice { get; set; }
    
    public Order Order { get; set; } = null!;
    public Product Product { get; set; } = null!;
}