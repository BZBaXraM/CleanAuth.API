namespace CleanAuth.Application.DTOs;

/// <summary>
/// Response model for successful user login
/// </summary>
public class LoginResponse
{
    /// <summary>
    /// JWT access token for authentication
    /// </summary>
    public string Token { get; set; } = string.Empty;

    /// <summary>
    /// Refresh token for obtaining new access tokens
    /// </summary>
    public string RefreshToken { get; set; } = string.Empty;

    /// <summary>
    /// Expiration time of the refresh token
    /// </summary>
    public DateTime? RefreshTokenExpireTime { get; set; }

    /// <summary>
    /// Initializes a new instance of the LoginResponse class
    /// </summary>
    public LoginResponse(string token, string refreshToken, DateTime? refreshTokenExpireTime)
    {
        Token = token;
        RefreshToken = refreshToken;
        RefreshTokenExpireTime = refreshTokenExpireTime;
    }
}
