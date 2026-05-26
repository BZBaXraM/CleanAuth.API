namespace CleanAuth.Application.DTOs;

/// <summary>
/// Request model for user registration
/// </summary>
public class RegisterRequest
{
    /// <summary>
    /// User's email address (must be unique and valid)
    /// </summary>
    public required string Email { get; set; }

    /// <summary>
    /// User's username (3-20 characters, letters, digits, and underscores only)
    /// </summary>
    public required string Username { get; set; }

    /// <summary>
    /// User's password (minimum 8 characters, must contain uppercase, lowercase, and digit)
    /// </summary>
    public required string Password { get; set; }

    /// <summary>
    /// User's gender (0 = Male, 1 = Female, 2 = Other)
    /// </summary>
    public Gender Gender { get; set; }

    /// <summary>
    /// User's date of birth (optional)
    /// </summary>
    public DateTime? DateOfBirth { get; set; }
}
