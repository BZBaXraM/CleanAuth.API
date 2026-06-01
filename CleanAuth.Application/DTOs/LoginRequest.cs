namespace CleanAuth.Application.DTOs;

/// <summary>
/// Request model for user login
/// </summary>
public class LoginRequest
{
    /// <summary>
    /// Username or email address of the user
    /// </summary>
    public required string UsernameOrEmail { get; set; }

    /// <summary>
    /// User's password
    /// </summary>
    public required string Password { get; set; }

    /// <summary>
    /// When true, the access token is valid for 30 days instead of the default expiry
    /// </summary>
    public bool RememberMe { get; set; }
}
