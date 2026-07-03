namespace CleanAuth.Application.DTOs;

/// <summary>
/// Request model for changing the password of the currently authenticated user
/// </summary>
public class ChangePasswordRequest
{
    /// <summary>
    /// The user's current password
    /// </summary>
    public string CurrentPassword { get; set; } = null!;

    /// <summary>
    /// New password (8-30 characters, must contain uppercase, lowercase, and digit)
    /// </summary>
    public string NewPassword { get; set; } = null!;
}
