namespace CleanAuth.Application.DTOs;

/// <summary>
/// Request model for initiating password reset process
/// </summary>
public class ForgetPasswordRequest
{
    /// <summary>
    /// Email address of the user requesting password reset
    /// </summary>
    public required string Email { get; set; }
}
