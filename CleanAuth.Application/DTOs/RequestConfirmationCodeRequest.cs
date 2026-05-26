namespace CleanAuth.Application.DTOs;

/// <summary>
/// Request model for requesting a new email confirmation code
/// </summary>
public class RequestConfirmationCodeRequest
{
    /// <summary>
    /// Email address to send the confirmation code to
    /// </summary>
    public required string Email { get; set; }
}
