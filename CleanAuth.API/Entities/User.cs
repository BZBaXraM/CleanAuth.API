namespace CleanAuth.API.Entities;

public class User
{
    /// <summary>
    /// The id of the user.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// The email of the user.
    /// </summary>
    public required string Email { get; set; }

    /// <summary>
    /// The login username of the user.
    /// </summary>
    public required string UserName { get; set; }

    /// <summary>
    /// The password of the user.
    /// </summary>
    public required string Password { get; set; }

    /// <summary>
    /// The email of the user.
    /// </summary>
    public DateTime DateOfBirth { get; set; }

    public required Gender Gender { get; set; }
    public string? Token { get; set; }
    public string? RefreshToken { get; set; }
    public DateTime? EmailConfirmationCodeExpireTime { get; set; }
    public bool IsEmailConfirmed { get; set; }
    public string? EmailConfirmationCode { get; set; }
    public DateTime RefreshTokenExpireTime { get; set; }
}