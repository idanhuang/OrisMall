using Microsoft.Extensions.Configuration;
using Moq;
using OrisMall.Core.DTOs;
using OrisMall.Core.Entities;
using OrisMall.Core.Interfaces;
using OrisMall.Core.Exceptions;
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
    public async Task RegisterAsync_ShouldReturnUserDto_WhenValidData()
    {
        // Arrange
        var registerDto = new RegisterDto
        {
            Email = "dan@example.com",
            FirstName = "Dan",
            LastName = "Huang",
            Password = "Password123!",
            PhoneNumber = "+1234567890"
        };

        var expectedUser = new User
        {
            Id = 1,
            Email = registerDto.Email,
            FirstName = registerDto.FirstName,
            LastName = registerDto.LastName,
            PhoneNumber = registerDto.PhoneNumber,
            Role = "User",
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        _mockUserRepository.Setup(r => r.EmailExistsAsync(registerDto.Email)).ReturnsAsync(false);
        _mockUserRepository.Setup(r => r.AddAsync(It.IsAny<User>())).ReturnsAsync(expectedUser);

        // Act
        var result = await _userService.RegisterAsync(registerDto);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(expectedUser.Id, result.Id);
        Assert.Equal(expectedUser.Email, result.Email);
        Assert.Equal("User", result.Role);
        Assert.True(result.IsActive);
    }

    [Fact]
    public async Task RegisterAsync_ShouldThrowValidationException_WhenEmailExists()
    {
        // Arrange
        var registerDto = new RegisterDto
        {
            Email = "dan@example.com",
            FirstName = "Dan",
            LastName = "Huang",
            Password = "Password123!",
            PhoneNumber = "+1234567890"
        };

        _mockUserRepository.Setup(r => r.EmailExistsAsync(registerDto.Email)).ReturnsAsync(true);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<ValidationException>(() => _userService.RegisterAsync(registerDto));
        Assert.Equal("Email already exists", exception.Message);
    }

    [Fact]
    public async Task LoginAsync_ShouldReturnAuthResponse_WhenValidCredentials()
    {
        // Arrange
        var mockUser = MockDataHelper.GetMockUsers().First();
        var loginDto = new LoginDto { Email = mockUser.Email, Password = "password123" };

        _mockUserRepository.Setup(r => r.GetByEmailAsync(loginDto.Email)).ReturnsAsync(mockUser);

        // Act
        var result = await _userService.LoginAsync(loginDto);

        // Assert
        Assert.NotNull(result);
        Assert.NotNull(result.Token);
        Assert.Equal(mockUser.Id, result.User.Id);
        Assert.Equal(mockUser.Email, result.User.Email);
    }

    [Fact]
    public async Task LoginAsync_ShouldThrowUnauthorizedException_WhenInvalidCredentials()
    {
        // Arrange
        var loginDto = new LoginDto { Email = "notexist@example.com", Password = "Password123!" };

        _mockUserRepository.Setup(r => r.GetByEmailAsync(loginDto.Email)).ReturnsAsync((User?)null);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<UnauthorizedException>(() => _userService.LoginAsync(loginDto));
        Assert.Equal("Invalid email or password", exception.Message);
    }

    [Fact]
    public async Task GetCurrentUserAsync_ShouldReturnUserDto_WhenUserExists()
    {
        // Arrange
        var userId = 1;
        var mockUser = new User
        {
            Id = userId,
            Email = "user@example.com",
            FirstName = "Dan",
            LastName = "Huang",
            PhoneNumber = "+1234567890",
            Role = "User",
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        _mockUserRepository.Setup(r => r.GetByIdAsync(userId)).ReturnsAsync(mockUser);

        // Act
        var result = await _userService.GetCurrentUserAsync(userId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(mockUser.Id, result.Id);
        Assert.Equal(mockUser.Email, result.Email);
        Assert.Equal(mockUser.FirstName, result.FirstName);
        Assert.Equal(mockUser.LastName, result.LastName);
    }
}