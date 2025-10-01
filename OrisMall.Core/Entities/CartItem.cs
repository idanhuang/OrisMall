using System.ComponentModel.DataAnnotations;

namespace OrisMall.Core.Entities;

public class CartItem
{
    public int Id { get; set; }
    
    public int UserId { get; set; }
    
    public int ProductId { get; set; }
    
    [Range(1, int.MaxValue)]
    public int Quantity { get; set; }
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    public DateTime? UpdatedAt { get; set; }
    
    // Navigation properties
    public User User { get; set; } = null!;
    public Product Product { get; set; } = null!;
}


