using Microsoft.Extensions.Configuration;
using Moq;
using OrisMall.Core.DTOs;
using OrisMall.Core.Entities;
using OrisMall.Core.Interfaces;
using OrisMall.Infrastructure.Services;
using OrisMall.Tests.Helpers;

namespace OrisMall.Tests.Services;

public class UserServiceTests
{
    private readonly Mock<IUserRepository> _mockUserRepository;
    private readonly Mock<IConfiguration> _mockConfiguration;
    private readonly UserService _userService;

    public UserServiceTests()
    {
        _mockUserRepository = new Mock<IUserRepository>();
        _mockConfiguration = new Mock<IConfiguration>();
        
        // Setup JWT configuration
        _mockConfiguration.Setup(c => c["Jwt:Key"]).Returns("ThisIsAVeryLongSecretKeyForJWTTokenGeneration123456789");
        _mockConfiguration.Setup(c => c["Jwt:Issuer"]).Returns("OrisMall");
        _mockConfiguration.Setup(c => c["Jwt:Audience"]).Returns("OrisMallUsers");
        
        _userService = new UserService(_mockUserRepository.Object, _mockConfiguration.Object);
    }

    [Fact]
    public async Task LoginAsync_ShouldReturnAuthResponse_WhenValidCredentials()
    {
        // Arrange
        var mockUser = MockDataHelper.GetMockUsers().First();
        //Uses "password123" as the password to matches the mock user's hashW
        var loginDto = new LoginDto { Email = mockUser.Email, Password = "password123" }; 


        _mockUserRepository.Setup(r => r.GetByEmailAsync(loginDto.Email)).ReturnsAsync(mockUser);

        // Act
        var result = await _userService.LoginAsync(loginDto);

        // Assert
        Assert.NotNull(result);
        Assert.NotNull(result.Token);
        Assert.Equal(mockUser.Id, result.User.Id);
        Assert.Equal(mockUser.Email, result.User.Email);
        _mockUserRepository.Verify(r => r.GetByEmailAsync(loginDto.Email), Times.Once);
    }
}