using OrisMall.Core.DTOs;

namespace OrisMall.Tests.Services;

public class PaymentGatewayTests
{
    private readonly MockPaymentGateway.MockPaymentGateway _paymentService;

    public PaymentGatewayTests()
    {
        _paymentService = new MockPaymentGateway.MockPaymentGateway();
    }

    [Fact]
    public async Task ProcessPaymentAsync_ShouldReturnSuccessResponse_WhenPaymentIsValid()
    {
        var paymentRequest = new ProcessPaymentDto
        {
            OrderId = "order_test_1",
            Amount = 100.00m,
            Currency = "USD"
        };

        var response = await _paymentService.ProcessPaymentAsync(paymentRequest);

        Assert.NotNull(response);
        Assert.True(response.IsSuccess);
        Assert.Equal("succeeded", response.Status);
        Assert.NotNull(response.PaymentId);
        Assert.NotNull(response.GatewayPaymentId);
        Assert.Equal(100.00m, response.Amount);
        Assert.Equal("USD", response.Currency);
        Assert.True(response.CompletedAt > DateTime.MinValue);
    }

    [Fact]
    public async Task GetPaymentStatusAsync_ShouldReturnPaymentStatus_WhenPaymentExists()
    {
        // First create a payment
        var paymentRequest = new ProcessPaymentDto
        {
            OrderId = "order_test_2",
            Amount = 200.00m,
            Currency = "USD"
        };
        var paymentResponse = await _paymentService.ProcessPaymentAsync(paymentRequest);

        // Then get its status
        var status = await _paymentService.GetPaymentStatusAsync(paymentResponse.PaymentId);

        Assert.NotNull(status);
        Assert.Equal("succeeded", status.Status);
        Assert.Equal(paymentResponse.PaymentId, status.PaymentId);
        Assert.Equal(paymentResponse.GatewayPaymentId, status.GatewayPaymentId);
        Assert.Equal(200.00m, status.Amount);
        Assert.Equal("USD", status.Currency);
        Assert.True(status.InitiatedAt > DateTime.MinValue);
        Assert.True(status.CompletedAt > DateTime.MinValue);
    }

    [Fact]
    public async Task RefundPaymentAsync_ShouldReturnSuccessResponse_WhenPaymentExists()
    {
        // First create a payment
        var paymentRequest = new ProcessPaymentDto
        {
            OrderId = "order_test_3",
            Amount = 300.00m,
            Currency = "USD"
        };
        var paymentResponse = await _paymentService.ProcessPaymentAsync(paymentRequest);

        // Then refund it
        var refundRequest = new RefundPaymentDto
        {
            PaymentId = paymentResponse.PaymentId,
            Amount = 150.00m
        };

        var refundResponse = await _paymentService.RefundPaymentAsync(refundRequest);

        Assert.NotNull(refundResponse);
        Assert.True(refundResponse.IsSuccess);
        Assert.Equal("succeeded", refundResponse.Status);
        Assert.Equal(paymentResponse.GatewayPaymentId, refundResponse.PaymentId);
        Assert.Equal(150.00m, refundResponse.Amount);
        Assert.True(refundResponse.CompletedAt > DateTime.MinValue);
    }
}