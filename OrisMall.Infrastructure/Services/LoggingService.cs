using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using OrisMall.Core.Interfaces;
using Serilog;
using Serilog.Events;

namespace OrisMall.Infrastructure.Services;

public class LoggingService : ILoggingService
{
    private readonly ILogger<LoggingService> _logger;

    public LoggingService(ILogger<LoggingService> logger)
    {
        _logger = logger;
    }

    public Task LogRequestAsync(HttpContext context, string requestId)
    {
        try
        {
            var request = context.Request;
            var logData = new
            {
                RequestId = requestId,
                Method = request.Method,
                Path = request.Path.Value,
                QueryString = request.QueryString.Value,
                Headers = GetSafeHeaders(request.Headers),
                RemoteIpAddress = context.Connection.RemoteIpAddress?.ToString(),
                UserAgent = request.Headers.TryGetValue("User-Agent", out var userAgent) ? userAgent.ToString() : "Unknown",
                Timestamp = DateTime.UtcNow
            };

            Log.Information("=== INCOMING REQUEST === {@RequestData}", logData);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error logging request details for RequestId: {RequestId}", requestId);
        }
        
        return Task.CompletedTask;
    }

    public Task LogResponseAsync(HttpContext context, string requestId, long executionTimeMs)
    {
        try
        {
            var response = context.Response;
            var logData = new
            {
                RequestId = requestId,
                StatusCode = response.StatusCode,
                Headers = GetSafeHeaders(response.Headers),
                ExecutionTimeMs = executionTimeMs,
                Timestamp = DateTime.UtcNow
            };

            Log.Information("=== OUTGOING RESPONSE === {@ResponseData}", logData);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error logging response details for RequestId: {RequestId}", requestId);
        }
        
        return Task.CompletedTask;
    }

    public Task LogRequestSummaryAsync(string method, string path, int statusCode, long executionTimeMs, string requestId)
    {
        try
        {
            var logLevel = GetLogLevelForStatusCode(statusCode);
            var statusCategory = GetStatusCategory(statusCode);
            
            var serilogLevel = ConvertToSerilogLevel(logLevel);
            Log.Write(serilogLevel, 
                "=== REQUEST SUMMARY === [{RequestId}] {Method} {Path} -> {StatusCode} ({StatusCategory}) | {ExecutionTime}ms",
                requestId, method, path, statusCode, statusCategory, executionTimeMs);

            // Log performance warnings for slow requests
            if (executionTimeMs > 5000) // 5 seconds
            {
                Log.Warning("SLOW REQUEST DETECTED: [{RequestId}] {Method} {Path} took {ExecutionTime}ms", 
                    requestId, method, path, executionTimeMs);
            }
            else if (executionTimeMs > 1000) // 1 second
            {
                Log.Information("Performance Notice: [{RequestId}] {Method} {Path} took {ExecutionTime}ms", 
                    requestId, method, path, executionTimeMs);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error logging request summary for RequestId: {RequestId}", requestId);
        }
        
        return Task.CompletedTask;
    }

    private Dictionary<string, string> GetSafeHeaders(IHeaderDictionary headers)
    {
        // Not log sensitive header info for security reason.
        // TODO: In production, we can make them configurable.
        var safeHeaders = new Dictionary<string, string>();
        var sensitiveHeaders = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            "Authorization", "Cookie", "Set-Cookie", "X-API-Key", "X-Auth-Token", "Password"
        };

        foreach (var header in headers)
        {
            if (sensitiveHeaders.Contains(header.Key))
            {
                safeHeaders[header.Key] = "[REDACTED]";
            }
            else
            {
                safeHeaders[header.Key] = string.Join(", ", header.Value.ToArray());
            }
        }

        return safeHeaders;
    }

    private string GetStatusCategory(int statusCode)
    {
        return statusCode switch
        {
            >= 200 and < 300 => "Success",
            >= 300 and < 400 => "Redirection",
            >= 400 and < 500 => "Client Error",
            >= 500 => "Server Error",
            _ => "Unknown"
        };
    }

    private LogLevel GetLogLevelForStatusCode(int statusCode)
    {
        return statusCode switch
        {
            >= 200 and < 300 => LogLevel.Information,
            >= 300 and < 400 => LogLevel.Information,
            >= 400 and < 500 => LogLevel.Warning,
            >= 500 => LogLevel.Error,
            _ => LogLevel.Information
        };
    }

    private LogEventLevel ConvertToSerilogLevel(LogLevel logLevel)
    {
        return logLevel switch
        {
            LogLevel.Trace => LogEventLevel.Verbose,
            LogLevel.Debug => LogEventLevel.Debug,
            LogLevel.Information => LogEventLevel.Information,
            LogLevel.Warning => LogEventLevel.Warning,
            LogLevel.Error => LogEventLevel.Error,
            LogLevel.Critical => LogEventLevel.Fatal,
            LogLevel.None => LogEventLevel.Fatal,
            _ => LogEventLevel.Information
        };
    }
}