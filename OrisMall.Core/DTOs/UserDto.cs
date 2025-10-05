using System.ComponentModel.DataAnnotations;

namespace OrisMall.Core.DTOs;

public class UserDto
{
    public int Id { get; set; }
    
    [EmailAddress]
    [StringLength(100)]
    public string Email { get; set; } = string.Empty;
    
    [StringLength(20)]
    public string FirstName { get; set; } = string.Empty;
    
    [StringLength(20)]
    public string LastName { get; set; } = string.Empty;
    
    [StringLength(20)]
    [RegularExpression(@"^\+?[1-9]\d{1,14}$", ErrorMessage = "Invalid phone number format. Use international format. Valid example: +1234567890")]
    public string? PhoneNumber { get; set; }
    
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? LastLoginAt { get; set; }
    
    [StringLength(20)]
    public string Role { get; set; } = string.Empty;
}

public class RegisterDto
{
    [Required]
    [EmailAddress]
    [StringLength(100)]
    public string Email { get; set; } = string.Empty;
    
    [Required]
    [StringLength(20)]
    public string FirstName { get; set; } = string.Empty;
    
    [Required]
    [StringLength(20)]
    public string LastName { get; set; } = string.Empty;
    
    [Required]
    [StringLength(100, MinimumLength = 8)]
    [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]+$", ErrorMessage = "Password must contain at least 8 characters with uppercase, lowercase, number, and special character. Valid example: MyPass123!")]
    public string Password { get; set; } = string.Empty;
    
    [StringLength(20)]
    [RegularExpression(@"^\+?[1-9]\d{1,14}$", ErrorMessage = "Invalid phone number format. Use international format. Valid example: +1234567890")]
    public string? PhoneNumber { get; set; }
}

public class LoginDto
{
    [Required]
    [EmailAddress]
    [StringLength(100)]
    public string Email { get; set; } = string.Empty;
    
    [Required]
    public string Password { get; set; } = string.Empty;
}

public class AuthResponseDto
{
    public string Token { get; set; } = string.Empty;
    public UserDto User { get; set; } = null!;
    public DateTime ExpiresAt { get; set; }
}

public class UpdateProfileDto
{
    [StringLength(20)]
    public string? FirstName { get; set; }
    
    [StringLength(20)]
    public string? LastName { get; set; }
    
    [StringLength(20)]
    [RegularExpression(@"^\+?[1-9]\d{1,14}$", ErrorMessage = "Invalid phone number format. Use international format. Valid example: +1234567890")]
    public string? PhoneNumber { get; set; }
}

public class ChangePasswordDto
{
    [Required]
    public string CurrentPassword { get; set; } = "CurrentPass123!";
    
    [Required]
    [StringLength(100, MinimumLength = 8)]
    [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]+$", ErrorMessage = "Password must contain at least 8 characters with uppercase, lowercase, number, and special character. Valid example: MyPass123!")]
    public string NewPassword { get; set; } = "NewPass123!";
}