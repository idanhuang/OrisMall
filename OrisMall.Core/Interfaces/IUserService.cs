using OrisMall.Core.DTOs;

namespace OrisMall.Core.Interfaces;

public interface IUserService
{
    Task<UserDto> RegisterAsync(RegisterDto registerDto);
    Task<UserDto> CreateAdminAsync(RegisterDto registerDto);
    Task<UserDto> SetupFirstAdminAsync(RegisterDto registerDto);
    Task<AuthResponseDto> LoginAsync(LoginDto loginDto);
    
    // Profile management
    Task<UserDto> GetCurrentUserAsync(int userId);
}