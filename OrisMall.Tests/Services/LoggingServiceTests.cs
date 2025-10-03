using Microsoft.Extensions.Logging;
using Moq;
using OrisMall.Infrastructure.Services;
using OrisMall.Tests.Helpers;

namespace OrisMall.Tests.Services;

public class LoggingServiceTests
{
    private readonly Mock<ILogger<LoggingService>> _mockLogger;
    private readonly LoggingService _loggingService;

    public LoggingServiceTests()
    {
        _mockLogger = new Mock<ILogger<LoggingService>>();
        _loggingService = new LoggingService(_mockLogger.Object);
    }

    [Fact]
    public async Task LogRequestAsync_ShouldNotThrowException_WhenValidContext()
    {
        // Arrange
        var context = MockDataHelper.CreateMockHttpContext();
        var requestId = "test-request-123";

        // Act & Assert - Should not throw exception
        var exception = await Record.ExceptionAsync(() => 
            _loggingService.LogRequestAsync(context, requestId));
        
        Assert.Null(exception);
    }
}