using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using OrisMall.Core.DTOs;
using OrisMall.Core.Interfaces;
using OrisMall.Core.Exceptions;

namespace OrisMall.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly ILogger<AuthController> _logger;

    public AuthController(IUserService userService, ILogger<AuthController> logger)
    {
        _userService = userService;
        _logger = logger;
    }

    /// <summary>
    /// Register new user account
    /// Time Complexity: O(1) for database insert + O(k) for password hashing where k is password length
    /// Space Complexity: O(1) for single user creation
    /// </summary>
    /// <returns>Created user information</returns>
    [HttpPost("register")]
    public async Task<ActionResult<UserDto>> Register(RegisterDto registerDto)
    {
        _logger.LogInformation("Registering new user with email {Email}", registerDto.Email);
        
        var user = await _userService.RegisterAsync(registerDto);
        
        _logger.LogInformation("User registered successfully with ID {UserId}", user.Id);
        return CreatedAtAction(nameof(GetCurrentUser), new { }, user);
    }

    /// <summary>
    /// Authenticate user and return JWT token
    /// Time Complexity: O(1) for database lookup + O(k) for password verification where k is password length
    /// Space Complexity: O(1) for JWT token generation
    /// </summary>
    /// <returns>JWT token and user information</returns>
    [HttpPost("login")]
    public async Task<ActionResult<AuthResponseDto>> Login(LoginDto loginDto)
    {
        _logger.LogInformation("User login attempt for email {Email}", loginDto.Email);
        
        var authResponse = await _userService.LoginAsync(loginDto);
        
        _logger.LogInformation("User login successful for email {Email}", loginDto.Email);
        return Ok(authResponse);
    }

    /// <summary>
    /// Logout current user
    /// Time Complexity: O(1) for logging operation (JWT is stateless)
    /// Space Complexity: O(1) for simple response
    /// </summary>
    /// <returns>Success message</returns>
    [HttpPost("logout")]
    [Authorize]
    public async Task<ActionResult> Logout()
    {
        var userId = GetCurrentUserId();
        _logger.LogInformation("User logout for ID {UserId}", userId);

        // JWT tokens are stateless and can't be invalidated server-side.In production, use a blacklist or refresh tokens.
        // Here, we just log the logout.
        _logger.LogInformation("User logged out successfully for ID {UserId}", userId);

        return Ok(new { message = "Logged out successfully" });
    }


    /// <summary>
    /// Get current user profile
    /// Time Complexity: O(1) for database lookup by primary key
    /// Space Complexity: O(1) for single user data
    /// </summary>
    /// <returns>Current user information</returns>
    [HttpGet("profile/get-profile")]
    [Authorize]
    public async Task<ActionResult<UserDto>> GetCurrentUser()
    {
        var userId = GetCurrentUserId();
        _logger.LogInformation("Retrieving current user profile for ID {UserId}", userId);
        
        var user = await _userService.GetCurrentUserAsync(userId);
        
        _logger.LogInformation("Current user profile retrieved successfully for ID {UserId}", userId);
        return Ok(user);
    }

    /// <summary>
    /// Update current user profile
    /// Time Complexity: O(1) for database update by primary key
    /// Space Complexity: O(1) for single user update
    /// </summary>
    /// <returns>Updated user information</returns>
    [HttpPut("profile/update-profile")]
    [Authorize]
    public async Task<ActionResult<UserDto>> UpdateProfile(UpdateProfileDto updateDto)
    {
        // TODO: Add rate limiting (e.g., 1 requests per day)
        // TODO: Implement profile update functionality
        return Ok(new { message = "Profile update not implemented yet" });
    }

    /// <summary>
    /// Change current user password
    /// Time Complexity: O(1) for database update + O(k) for password hashing where k is password length
    /// Space Complexity: O(1) for single password update
    /// </summary>
    /// <returns>Success message</returns>
    [HttpPost("change-password")]
    [Authorize]
    public async Task<ActionResult> ChangePassword(ChangePasswordDto changePasswordDto)
    {
        // TODO: Add rate limiting (e.g., 1 requests per day)
        // TODO: Implement password change functionality
        return Ok(new { message = "Password change not implemented yet" });
    }

    /// <summary>
    /// Create new admin user (admin only)
    /// Time Complexity: O(1) for database insert + O(k) for password hashing where k is password length
    /// Space Complexity: O(1) for single admin user creation
    /// </summary>
    /// <returns>Created admin user</returns>
    [HttpPost("admin")]
    [Authorize(Policy = "AdminOnly")]
    public async Task<ActionResult<UserDto>> CreateAdmin(RegisterDto registerDto)
    {
        _logger.LogInformation("Creating admin user with email {Email}", registerDto.Email);
        
        var user = await _userService.CreateAdminAsync(registerDto);
        
        _logger.LogInformation("Admin user created successfully with ID {UserId}", user.Id);
        return CreatedAtAction(nameof(GetCurrentUser), new { }, user);
    }

    /// <summary>
    /// Setup first admin account (bootstrap)
    /// Time Complexity: O(n) for checking existing admins + O(1) for insert + O(k) for password hashing
    /// Space Complexity: O(1) for single admin user creation
    /// </summary>
    /// <returns>Created first admin user</returns>
    [HttpPost("setup-super-admin")]
    public async Task<ActionResult<UserDto>> SetupFirstAdmin(RegisterDto registerDto)
    {
        _logger.LogInformation("Setting up first admin user with email {Email}", registerDto.Email);
        
        var user = await _userService.SetupFirstAdminAsync(registerDto);
        
        _logger.LogInformation("First admin user setup successfully with ID {UserId}", user.Id);
        return CreatedAtAction(nameof(GetCurrentUser), new { }, user);
    }

    /// <summary>
    /// Get current user ID from JWT token
    /// </summary>
    /// <returns>User ID from token</returns>
    /// <exception cref="UnauthorizedException">Invalid or missing user ID claim</exception>
    private int GetCurrentUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out var userId))
        {
            throw new UnauthorizedException("Invalid user token");
        }
        return userId;
    }
}