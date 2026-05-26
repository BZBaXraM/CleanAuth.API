namespace CleanAuth.Domain.Entities;

/// <summary>
/// Represents a user entity in the authentication system.
/// </summary>
public class User : BaseEntity
{
    /// <summary>
    /// Gets or sets the email address of the user.
    /// </summary>
    public required string Email { get; set; }

    /// <summary>
    /// Gets or sets the username of the user.
    /// </summary>
    public required string UserName { get; set; }

    /// <summary>
    /// Gets or sets the password of the user.
    /// </summary>
    public required string Password { get; set; }

    /// <summary>
    /// Gets or sets the gender of the user.
    /// </summary>
    public Gender Gender { get; set; }

    /// <summary>
    /// Gets or sets the date of birth of the user.
    /// </summary>
    public DateTime? DateOfBirth { get; set; }

    /// <summary>
    /// Gets or sets the authentication token for the user.
    /// </summary>
    public string? Token { get; set; }

    /// <summary>
    /// Gets or sets the refresh token for the user.
    /// </summary>
    public string? RefreshToken { get; set; }

    /// <summary>
    /// Gets or sets the expiration time for the email confirmation code.
    /// </summary>
    public DateTime? EmailConfirmationCodeExpireTime { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the user's email has been confirmed.
    /// </summary>
    public bool IsEmailConfirmed { get; set; }

    /// <summary>
    /// Gets or sets the email confirmation code sent to the user.
    /// </summary>
    public string? EmailConfirmationCode { get; set; }

    /// <summary>
    /// Gets or sets the expiration time for the refresh token.
    /// </summary>
    public DateTime RefreshTokenExpireTime { get; set; }

    /// <summary>
    /// Gets or sets the password reset code sent to the user.
    /// </summary>
    public string? PasswordResetCode { get; set; }

    /// <summary>
    /// Gets or sets the expiration time for the password reset code.
    /// </summary>
    public DateTime? PasswordResetCodeExpireTime { get; set; }
}
