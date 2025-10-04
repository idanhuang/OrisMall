using OrisMall.Core.DTOs;

namespace OrisMall.Core.Interfaces;

public interface IPaymentService
{
    Task<PaymentResponseDto> ProcessPaymentAsync(ProcessPaymentDto paymentRequest);
    Task<PaymentStatusDto> GetPaymentStatusAsync(string paymentId);
    Task<RefundResponseDto> RefundPaymentAsync(RefundPaymentDto refundRequest);
}