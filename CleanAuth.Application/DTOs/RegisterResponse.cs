namespace CleanAuth.Application.DTOs;

/// <summary>
/// Response model for user registration
/// </summary>
public class RegisterResponse
{
    /// <summary>
    /// Indicates whether the registration was successful
    /// </summary>
    public bool Success { get; set; }

    /// <summary>
    /// Message describing the registration result
    /// </summary>
    public string Message { get; set; }

    /// <summary>
    /// Email address of the registered user (if successful)
    /// </summary>
    public string? Email { get; set; }

    /// <summary>
    /// Initializes a new instance of the RegisterResponse class
    /// </summary>
    public RegisterResponse(bool success, string message, string? email = null)
    {
        Success = success;
        Message = message;
        Email = email;
    }
}
