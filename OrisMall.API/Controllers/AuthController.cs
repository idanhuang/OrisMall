using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using OrisMall.Core.DTOs;
using OrisMall.Core.Interfaces;

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

    [HttpPost("register")]
    public async Task<ActionResult<UserDto>> Register(RegisterDto registerDto)
    {
        if (!ModelState.IsValid)
        {
            _logger.LogWarning("Invalid user registration request model state");
            return BadRequest(ModelState);
        }

        try
        {
            _logger.LogInformation("Registering new user with email {Email}", registerDto.Email);
            
            var user = await _userService.RegisterAsync(registerDto);
            
            _logger.LogInformation("User registered successfully with ID {UserId}", user.Id);
            return CreatedAtAction(nameof(GetUser), new { id = user.Id }, user);
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning("User registration failed: {Message}", ex.Message);
            return BadRequest(ex.Message);
        }
    }

    [HttpPost("login")]
    public async Task<ActionResult<AuthResponseDto>> Login(LoginDto loginDto)
    {
        if (!ModelState.IsValid)
        {
            _logger.LogWarning("Invalid login request model state");
            return BadRequest(ModelState);
        }

        try
        {
            _logger.LogInformation("User login attempt for email {Email}", loginDto.Email);
            
            var authResponse = await _userService.LoginAsync(loginDto);
            
            _logger.LogInformation("User login successful for email {Email}", loginDto.Email);
            return Ok(authResponse);
        }
        catch (UnauthorizedAccessException ex)
        {
            _logger.LogWarning("Login failed for email {Email}: {Message}", loginDto.Email, ex.Message);
            return Unauthorized(ex.Message);
        }
    }

    [HttpGet("user/{id}")]
    public async Task<ActionResult<UserDto>> GetUser(int id)
    {
        var user = await _userService.GetUserByIdAsync(id);
        if (user == null)
            return NotFound();

        return Ok(user);
    }

    [HttpGet("email-exists")]
    public async Task<ActionResult<bool>> EmailExists([FromQuery] string email)
    {
        var exists = await _userService.EmailExistsAsync(email);
        return Ok(exists);
    }

    [HttpPost("create-admin")]
    [Authorize(Policy = "AdminOnly")]
    public async Task<ActionResult<UserDto>> CreateAdmin(RegisterDto registerDto)
    {
        if (!ModelState.IsValid)
        {
            _logger.LogWarning("Invalid admin creation request model state");
            return BadRequest(ModelState);
        }

        try
        {
            _logger.LogInformation("Creating admin user with email {Email}", registerDto.Email);
            
            var user = await _userService.CreateAdminAsync(registerDto);
            
            _logger.LogInformation("Admin user created successfully with ID {UserId}", user.Id);
            return CreatedAtAction(nameof(GetUser), new { id = user.Id }, user);
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning("Admin creation failed: {Message}", ex.Message);
            return BadRequest(ex.Message);
        }
    }

    [HttpPost("setup-first-admin")]
    public async Task<ActionResult<UserDto>> SetupFirstAdmin(RegisterDto registerDto)
    {
        if (!ModelState.IsValid)
        {
            _logger.LogWarning("Invalid first admin setup request model state");
            return BadRequest(ModelState);
        }

        try
        {
            _logger.LogInformation("Setting up first admin user with email {Email}", registerDto.Email);
            
            var user = await _userService.SetupFirstAdminAsync(registerDto);
            
            _logger.LogInformation("First admin user setup successfully with ID {UserId}", user.Id);
            return CreatedAtAction(nameof(GetUser), new { id = user.Id }, user);
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning("First admin setup failed: {Message}", ex.Message);
            return BadRequest(ex.Message);
        }
    }
}