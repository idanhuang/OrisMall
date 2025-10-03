using OrisMall.Core.Interfaces;
using System.Diagnostics;

namespace OrisMall.API.Middleware;

public class RequestResponseLoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<RequestResponseLoggingMiddleware> _logger;

    public RequestResponseLoggingMiddleware(RequestDelegate next, ILogger<RequestResponseLoggingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context, ILoggingService loggingService)
    {
        // Generate unique request ID for tracking
        var requestId = Guid.NewGuid().ToString("N")[..8];
        context.Items["RequestId"] = requestId;

        // Start timing the request
        var stopwatch = Stopwatch.StartNew();

        try
        {
            // Log incoming request details
            await loggingService.LogRequestAsync(context, requestId);

            // Continue to next middleware
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled exception occurred during request processing. RequestId: {RequestId}", requestId);
            
            // Set error response if not already set
            if (!context.Response.HasStarted)
            {
                context.Response.StatusCode = 500;
                context.Response.ContentType = "application/json";
                
                var errorResponse = new
                {
                    error = "An internal server error occurred",
                    requestId = requestId,
                    timestamp = DateTime.UtcNow
                };
                
                await context.Response.WriteAsync(System.Text.Json.JsonSerializer.Serialize(errorResponse));
            }
        }
        finally
        {
            // Stop timing and log response time
            stopwatch.Stop();
            var executionTime = stopwatch.ElapsedMilliseconds;

            try
            {
                // Log outgoing response details
                await loggingService.LogResponseAsync(context, requestId, executionTime);

                // Log request summary details
                await loggingService.LogRequestSummaryAsync(
                    context.Request.Method,
                    context.Request.Path.Value ?? "",
                    context.Response.StatusCode,
                    executionTime,
                    requestId);
            }
            catch (Exception logEx)
            {
                _logger.LogError(logEx, "Error occurred while logging response details. RequestId: {RequestId}", requestId);
            }
        }
    }
}