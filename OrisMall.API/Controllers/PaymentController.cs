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
        if (!ModelState.IsValid)
        {
            _logger.LogWarning("Invalid payment request model state");
            return BadRequest(ModelState);
        }

        try
        {
            _logger.LogInformation("Processing payment request for order {OrderId}", paymentRequest.OrderId);

            var response = await _paymentService.ProcessPaymentAsync(paymentRequest);
            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing payment for order {OrderId}", paymentRequest.OrderId);
            return StatusCode(500, new { message = "Payment processing error", error = ex.Message });
        }
    }

    [HttpGet("status/{paymentId}")]
    public async Task<ActionResult<PaymentStatusDto>> GetPaymentStatus(string paymentId)
    {
        try
        {
            _logger.LogInformation("Retrieving payment status for {PaymentId}", paymentId);

            var status = await _paymentService.GetPaymentStatusAsync(paymentId);
            return Ok(status);
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning("Payment {PaymentId} not found: {Message}", paymentId, ex.Message);
            return NotFound(new { message = $"Payment with ID {paymentId} not found.", error = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving payment status for {PaymentId}", paymentId);
            return StatusCode(500, new { message = "Error retrieving payment status", error = ex.Message });
        }
    }

    [HttpPost("refund")]
    public async Task<ActionResult<RefundResponseDto>> RefundPayment([FromBody] RefundPaymentDto refundRequest)
    {
        if (!ModelState.IsValid)
        {
            _logger.LogWarning("Invalid refund request model state");
            return BadRequest(ModelState);
        }

        try
        {
            _logger.LogInformation("Processing refund request for payment {PaymentId}", refundRequest.PaymentId);

            var response = await _paymentService.RefundPaymentAsync(refundRequest);
            return Ok(response);
        }
        catch (ArgumentException ex)
        {
            // Better way is to let Paymentgateway throws different custom exception based on HTTP status code
            // For simplicity, I just check the error message.
            if (ex.Message.Contains("not found"))
            {
                _logger.LogWarning("Payment {PaymentId} not found for refund: {Message}", refundRequest.PaymentId, ex.Message);
                return NotFound(new { message = $"Payment with ID {refundRequest.PaymentId} not found for refund.", error = ex.Message });
            }
            else
            {
                _logger.LogWarning("Refund validation failed for payment {PaymentId}: {Message}", refundRequest.PaymentId, ex.Message);
                return BadRequest(new { message = "Refund validation failed", error = ex.Message });
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing refund for payment {PaymentId}", refundRequest.PaymentId);
            return StatusCode(500, new { message = "Refund processing error", error = ex.Message });
        }
    }
}