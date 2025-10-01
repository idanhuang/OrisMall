using System.ComponentModel.DataAnnotations;

namespace OrisMall.Core.Entities;

public class Order
{
    public int Id { get; set; }
    
    [Required]
    [StringLength(50)]
    public string OrderNumber { get; set; } = string.Empty;
    
    public int UserId { get; set; }
    
    [Required]
    [StringLength(100)]
    public string ShippingAddress { get; set; } = string.Empty;
    
    [Required]
    [StringLength(100)]
    public string BillingAddress { get; set; } = string.Empty;
    
    [Required]
    [StringLength(50)]
    public string Status { get; set; } = "Pending";
    
    [Range(0, double.MaxValue)]
    public decimal Subtotal { get; set; }
    
    [Range(0, double.MaxValue)]
    public decimal TaxAmount { get; set; }
    
    [Range(0, double.MaxValue)]
    public decimal ShippingCost { get; set; }
    
    [Range(0, double.MaxValue)]
    public decimal TotalAmount { get; set; }
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    public DateTime? UpdatedAt { get; set; }
    
    public DateTime? ShippedAt { get; set; }
    
    public DateTime? DeliveredAt { get; set; }
    
    public User User { get; set; } = null!;
    public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
}