using System.Net;
using System.Text.Json;
using OrisMall.Core.DTOs;
using OrisMall.Core.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace OrisMall.API.Middleware;

/// <summary>
/// Global exception handling middleware for error responses
/// </summary>
public class GlobalExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionHandlingMiddleware> _logger;
    private readonly IWebHostEnvironment _environment;

    public GlobalExceptionHandlingMiddleware(
        RequestDelegate next, 
        ILogger<GlobalExceptionHandlingMiddleware> logger,
        IWebHostEnvironment environment)
    {
        _next = next;
        _logger = logger;
        _environment = environment;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var requestId = context.Items["RequestId"]?.ToString() ?? Guid.NewGuid().ToString("N")[..8];
        var path = context.Request.Path.Value ?? "";
        var method = context.Request.Method;

        _logger.LogError(exception, "Unhandled exception occurred. RequestId: {RequestId}, Path: {Path}, Method: {Method}", 
            requestId, path, method);

        var errorResponse = CreateErrorResponse(exception, requestId, path, method);
        
        context.Response.StatusCode = errorResponse.ErrorCode switch
        {
            "NOT_FOUND" => 404,
            "UNAUTHORIZED" => 401,
            "FORBIDDEN" => 403,
            "VALIDATION_ERROR" => 400,
            "BUSINESS_ERROR" => 400,
            "DATABASE_ERROR" => 500,
            "EXTERNAL_SERVICE_ERROR" => 502,
            _ => 500
        };

        context.Response.ContentType = "application/json";

        var jsonResponse = JsonSerializer.Serialize(errorResponse, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = true
        });

        await context.Response.WriteAsync(jsonResponse);
    }

    private ErrorResponseDto CreateErrorResponse(Exception exception, string requestId, string path, string method)
    {
        return exception switch
        {
            // HTTP 400 - Bad Request
            ValidationException validationEx => new ErrorResponseDto
            {
                Error = "VALIDATION_ERROR",
                Message = validationEx.Message,
                ValidationErrors = validationEx.Errors,
                RequestId = requestId,
                Path = path,
                Method = method
            },
            
            // HTTP 404 - Not Found
            NotFoundException notFoundEx => new ErrorResponseDto
            {
                Error = "NOT_FOUND",
                Message = notFoundEx.Message,
                RequestId = requestId,
                Path = path,
                Method = method
            },
            
            // HTTP 401 - Unauthorized
            UnauthorizedException unauthorizedEx => new ErrorResponseDto
            {
                Error = "UNAUTHORIZED",
                Message = unauthorizedEx.Message,
                RequestId = requestId,
                Path = path,
                Method = method
            },
            
            // HTTP 403 - Forbidden
            ForbiddenException forbiddenEx => new ErrorResponseDto
            {
                Error = "FORBIDDEN",
                Message = forbiddenEx.Message,
                RequestId = requestId,
                Path = path,
                Method = method
            },
            
            // HTTP 400 - Bad Request (default for BusinessException)
            BusinessException businessEx => new ErrorResponseDto
            {
                Error = businessEx.ErrorCode,
                Message = businessEx.Message,
                RequestId = requestId,
                Path = path,
                Method = method
            },
            
            // HTTP 500 - Internal Server Error
            DbUpdateException dbEx => new ErrorResponseDto
            {
                Error = "DATABASE_ERROR",
                Message = "A database error occurred while processing your request.",
                Details = _environment.IsDevelopment() ? dbEx.InnerException?.Message : null,
                RequestId = requestId,
                Path = path,
                Method = method
            },
            
            // HTTP 500 - Internal Server Error
            TimeoutException timeoutEx => new ErrorResponseDto
            {
                Error = "TIMEOUT_ERROR",
                Message = "The request timed out. Please try again.",
                Details = _environment.IsDevelopment() ? timeoutEx.Message : null,
                RequestId = requestId,
                Path = path,
                Method = method
            },
            
            // HTTP 502 - Bad Gateway
            HttpRequestException httpEx => new ErrorResponseDto
            {
                Error = "EXTERNAL_SERVICE_ERROR",
                Message = "An external service is currently unavailable. Please try again later.",
                Details = _environment.IsDevelopment() ? httpEx.Message : null,
                RequestId = requestId,
                Path = path,
                Method = method
            },
            
            // HTTP 500 - Internal Server Error
            _ => new ErrorResponseDto
            {
                Error = "INTERNAL_SERVER_ERROR",
                Message = "An unexpected error occurred while processing your request.",
                Details = _environment.IsDevelopment() ? exception.Message : null,
                RequestId = requestId,
                Path = path,
                Method = method
            }
        };
    }
}