using Microsoft.AspNetCore.Mvc;
using OrisMall.Core.DTOs;
using OrisMall.Core.Interfaces;

namespace OrisMall.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PaymentController : ControllerBase
{
    private readonly IPaymentService _paymentService;
    private readonly ILogger<PaymentController> _logger;

    public PaymentController(IPaymentService paymentService, ILogger<PaymentController> logger)
    {
        _paymentService = paymentService;
        _logger = logger;
    }

    [HttpPost("process")]
    public async Task<ActionResult<PaymentResponseDto>> ProcessPayment([FromBody] ProcessPaymentDto paymentRequest)
    {
        _logger.LogInformation("Processing payment request for order {OrderId}", paymentRequest.OrderId);

        var response = await _paymentService.ProcessPaymentAsync(paymentRequest);
        return Ok(response);
    }

    [HttpGet("status/{paymentId}")]
    public async Task<ActionResult<PaymentStatusDto>> GetPaymentStatus(string paymentId)
    {
        _logger.LogInformation("Retrieving payment status for {PaymentId}", paymentId);

        var status = await _paymentService.GetPaymentStatusAsync(paymentId);
        return Ok(status);
    }

    [HttpPost("refund")]
    public async Task<ActionResult<RefundResponseDto>> RefundPayment([FromBody] RefundPaymentDto refundRequest)
    {
        _logger.LogInformation("Processing refund request for payment {PaymentId}", refundRequest.PaymentId);

        var response = await _paymentService.RefundPaymentAsync(refundRequest);
        return Ok(response);
    }
}