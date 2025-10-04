namespace OrisMall.Core.DTOs;

/// <summary>
/// Standardized error response structure
/// </summary>
public class ErrorResponseDto
{
    public string Error { get; set; } = string.Empty;
    public string ErrorCode { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public string? Details { get; set; }
    public Dictionary<string, string[]>? ValidationErrors { get; set; }
    public string RequestId { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; }
    public string Path { get; set; } = string.Empty;
    public string Method { get; set; } = string.Empty;

    public ErrorResponseDto()
    {
        Timestamp = DateTime.UtcNow;
    }

    public ErrorResponseDto(string error, string message, string requestId, string path, string method)
    {
        Error = error;
        Message = message;
        RequestId = requestId;
        Path = path;
        Method = method;
        Timestamp = DateTime.UtcNow;
    }
}
