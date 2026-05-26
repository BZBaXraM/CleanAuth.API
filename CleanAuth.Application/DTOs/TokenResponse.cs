namespace CleanAuth.Application.DTOs;

/// <summary>
/// Data transfer object for JWT tokens
/// </summary>
public class TokenResponse
{
    /// <summary>
    /// JWT access token
    /// </summary>
    public string Token { get; set; } = null!;

    /// <summary>
    /// Refresh token for obtaining new access tokens
    /// </summary>
    public string? RefreshToken { get; set; }

    /// <summary>
    /// Expiration time of the refresh token
    /// </summary>
    public DateTime RefreshTokenExpireTime { get; set; }
}
