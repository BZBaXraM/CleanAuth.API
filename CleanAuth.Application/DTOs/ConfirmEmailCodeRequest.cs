namespace CleanAuth.Application.DTOs;

/// <summary>
/// Request model for email confirmation using a verification code
/// </summary>
public class ConfirmEmailCodeRequest
{
    /// <summary>
    /// 6-character email confirmation code sent to the user's email
    /// </summary>
    public required string Code { get; set; }
}
