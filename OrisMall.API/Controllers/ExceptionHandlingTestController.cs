using Microsoft.AspNetCore.Mvc;
using OrisMall.Core.Exceptions;

namespace OrisMall.API.Controllers;

/// <summary>
/// Exception handling test controller to demonstrate global exception handling
/// Only available in DEBUG builds - automatically excluded from production
/// For demonstration purposes, it tests only some critical exceptions.
/// </summary>
#if DEBUG
[ApiController]
[Route("api/test/exception-handling")]
public class ExceptionHandlingTestController : ControllerBase
{
    private readonly ILogger<ExceptionHandlingTestController> _logger;

    public ExceptionHandlingTestController(ILogger<ExceptionHandlingTestController> logger)
    {
        _logger = logger;
    }

    [HttpGet("business-error")]
    public IActionResult TestBusinessError()
    {
        _logger.LogInformation("Testing business error exception");
        throw new BusinessException("This is a test business error", "TEST_BUSINESS_ERROR", 400);
    }

    [HttpGet("validation-error")]
    public IActionResult TestValidationError()
    {
        _logger.LogInformation("Testing validation error exception");
        var errors = new Dictionary<string, string[]>
        {
            { "Email", new[] { "Email is required", "Email format is invalid" } },
            { "Password", new[] { "Password must be at least 8 characters" } }
        };
        throw new ValidationException("Validation failed", errors);
    }

    [HttpGet("not-found-error")]
    public IActionResult TestNotFoundError()
    {
        _logger.LogInformation("Testing not found error exception");
        throw new NotFoundException("Product", 123);
    }

    [HttpGet("unauthorized-error")]
    public IActionResult TestUnauthorizedError()
    {
        _logger.LogInformation("Testing unauthorized error exception");
        throw new UnauthorizedException("Invalid credentials provided");
    }

    [HttpGet("forbidden-error")]
    public IActionResult TestForbiddenError()
    {
        _logger.LogInformation("Testing forbidden error exception");
        throw new ForbiddenException("You don't have permission to access this resource");
    }
}
#endif