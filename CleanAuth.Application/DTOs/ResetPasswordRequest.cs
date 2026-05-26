namespace CleanAuth.Application.DTOs;

/// <summary>
/// Request model for resetting user password using a verification code
/// </summary>
public class ResetPasswordRequest
{
    /// <summary>
    /// Email address of the user
    /// </summary>
    public string Email { get; set; } = null!;

    /// <summary>
    /// 6-character password reset code sent to the user's email
    /// </summary>
    public string Code { get; set; } = null!;

    /// <summary>
    /// New password for the user (minimum 8 characters, must contain uppercase, lowercase, and digit)
    /// </summary>
    public string NewPassword { get; set; } = null!;
}
