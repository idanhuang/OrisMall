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

    public AuthController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpPost("register")]
    public async Task<ActionResult<UserDto>> Register(RegisterDto registerDto)
    {
        try
        {
            var user = await _userService.RegisterAsync(registerDto);
            return CreatedAtAction(nameof(GetUser), new { id = user.Id }, user);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPost("login")]
    public async Task<ActionResult<AuthResponseDto>> Login(LoginDto loginDto)
    {
        try
        {
            var authResponse = await _userService.LoginAsync(loginDto);
            return Ok(authResponse);
        }
        catch (UnauthorizedAccessException ex)
        {
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
        try
        {
            var user = await _userService.CreateAdminAsync(registerDto);
            return CreatedAtAction(nameof(GetUser), new { id = user.Id }, user);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPost("setup-first-admin")]
    public async Task<ActionResult<UserDto>> SetupFirstAdmin(RegisterDto registerDto)
    {
        try
        {
            var user = await _userService.SetupFirstAdminAsync(registerDto);
            return CreatedAtAction(nameof(GetUser), new { id = user.Id }, user);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }
}