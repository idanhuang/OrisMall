using System.ComponentModel.DataAnnotations;

namespace OrisMall.Core.DTOs;

public class ProcessPaymentDto
{
    [Required]
    public string OrderId { get; set; } = string.Empty;
    
    [Required]
    [Range(0.01, double.MaxValue, ErrorMessage = "Amount must be greater than 0")]
    public decimal Amount { get; set; }
    
    [Required]
    [RegularExpression(@"^[A-Z]{3}$", ErrorMessage = "Currency must be 3 uppercase letters. Valid example: USD")]
    public string Currency { get; set; } = "USD";
}

public class PaymentResponseDto
{
    public bool IsSuccess { get; set; }
    public string Status { get; set; } = string.Empty;
    public string PaymentId { get; set; } = string.Empty;
    public string? GatewayPaymentId { get; set; }
    public decimal Amount { get; set; }
    public string Currency { get; set; } = string.Empty;
    public DateTime CompletedAt { get; set; }
}

public class PaymentStatusDto
{
    public string Status { get; set; } = string.Empty;
    public string PaymentId { get; set; } = string.Empty;
    public string? GatewayPaymentId { get; set; }
    public decimal Amount { get; set; }
    public string Currency { get; set; } = string.Empty;
    public DateTime InitiatedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
    
    // Refund information
    public DateTime? RefundedAt { get; set; }
    public decimal? RefundedAmount { get; set; }
}

public class RefundPaymentDto
{
    [Required]
    public string PaymentId { get; set; } = string.Empty;
    
    [Range(0.01, double.MaxValue, ErrorMessage = "Amount must be greater than 0")]
    public decimal? Amount { get; set; }
}

public class RefundResponseDto
{
    public bool IsSuccess { get; set; }
    public string Status { get; set; } = string.Empty;
    public string RefundId { get; set; } = string.Empty;
    public string PaymentId { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public DateTime CompletedAt { get; set; }
}