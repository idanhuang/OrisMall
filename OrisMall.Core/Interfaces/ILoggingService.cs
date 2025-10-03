using Microsoft.AspNetCore.Http;

namespace OrisMall.Core.Interfaces;

public interface ILoggingService
{
    Task LogRequestAsync(HttpContext context, string requestId);

    Task LogResponseAsync(HttpContext context, string requestId, long executionTimeMs);

    Task LogRequestSummaryAsync(string method, string path, int statusCode, long executionTimeMs, string requestId);
}