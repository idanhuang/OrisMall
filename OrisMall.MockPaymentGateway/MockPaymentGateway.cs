using OrisMall.Core.DTOs;
using OrisMall.Core.Interfaces;

namespace OrisMall.MockPaymentGateway;

public class MockPaymentGateway : IPaymentService
{
    // Use Dictionary to mock the payment DB
    private static readonly Dictionary<string, PaymentStatusDto> _payments = new();

    public async Task<PaymentResponseDto> ProcessPaymentAsync(ProcessPaymentDto request)
    {
        // Simulate payment gateway takes 100ms to complete payment process
        var initiatedAt = DateTime.UtcNow;
        await Task.Delay(100);
        var completedAt = DateTime.UtcNow; 

        var paymentId = $"pay_{Guid.NewGuid():N}";
        var gatewayPaymentId = $"txn_{Guid.NewGuid():N}";

        var response = new PaymentResponseDto
        {
            IsSuccess = true,
            Status = "succeeded",
            PaymentId = paymentId,
            GatewayPaymentId = gatewayPaymentId,
            Amount = request.Amount,
            Currency = request.Currency,
            CompletedAt = completedAt
        };

        // Store for status lookup
        _payments[paymentId] = new PaymentStatusDto
        {
            Status = "succeeded",
            PaymentId = paymentId,
            GatewayPaymentId = gatewayPaymentId,
            Amount = request.Amount,
            Currency = request.Currency,
            InitiatedAt = initiatedAt,
            CompletedAt = completedAt
        };

        return response;
    }

    public async Task<PaymentStatusDto> GetPaymentStatusAsync(string paymentId)
    {
        // Simulate payment gateway takes 100ms to retrieve the payment status from DB
        await Task.Delay(100);

        if (_payments.TryGetValue(paymentId, out var status))
        {
            return status;
        }

        throw new ArgumentException($"Payment with ID {paymentId} not found.");
    }

    public async Task<RefundResponseDto> RefundPaymentAsync(RefundPaymentDto request)
    {
        // Check if payment exists first
        if (!_payments.TryGetValue(request.PaymentId, out var payment))
        {
            throw new ArgumentException($"Payment with ID {request.PaymentId} not found for refund.");
        }

        // Check if payment is already refunded
        if (payment.Status == "refunded")
        {
            throw new ArgumentException($"Payment {request.PaymentId} has already been refunded.");
        }

        // Simulate payment gateway takes 100ms to complete refund process
        await Task.Delay(100);
        var completedAt = DateTime.UtcNow;

        var refundId = $"ref_{Guid.NewGuid():N}";
        var refundAmount = request.Amount ?? payment.Amount;
        if (refundAmount > payment.Amount)
        {
            throw new ArgumentException($"Refund amount ({refundAmount}) cannot exceed original payment amount ({payment.Amount})");
        }

        var response = new RefundResponseDto
        {
            IsSuccess = true,
            Status = "succeeded",
            RefundId = refundId,
            PaymentId = payment.GatewayPaymentId ?? request.PaymentId,
            Amount = refundAmount,
            CompletedAt = completedAt
        };

        // Update payment status
        payment.Status = "refunded";
        payment.RefundedAt = completedAt;
        payment.RefundedAmount = refundAmount;

        return response;
    }
}