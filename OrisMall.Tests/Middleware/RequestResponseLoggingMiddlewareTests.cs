using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Moq;
using OrisMall.API.Middleware;
using OrisMall.Core.Interfaces;
using OrisMall.Tests.Helpers;

namespace OrisMall.Tests.Middleware;

public class RequestResponseLoggingMiddlewareTests
{
    [Fact]
    public async Task InvokeAsync_ShouldProcessRequestSuccessfully()
    {
        // Arrange
        var mockNext = new Mock<RequestDelegate>();
        var mockLogger = new Mock<ILogger<RequestResponseLoggingMiddleware>>();
        var mockLoggingService = new Mock<ILoggingService>();
        
        mockNext.Setup(n => n(It.IsAny<HttpContext>())).Returns(Task.CompletedTask);
        
        var middleware = new RequestResponseLoggingMiddleware(mockNext.Object, mockLogger.Object);
        var context = MockDataHelper.CreateMockHttpContext();

        // Act
        await middleware.InvokeAsync(context, mockLoggingService.Object);

        // Assert
        mockNext.Verify(n => n(It.IsAny<HttpContext>()), Times.Once);
    }
}