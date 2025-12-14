namespace CleanAuth.API.DTOs;

public class TokenDto
{
    /// <summary>
    /// Token
    /// </summary>
    public string Token { get; set; } = null!;

    /// <summary>
    /// Refresh token
    /// </summary>
    public string? RefreshToken { get; set; }

    /// <summary>
    /// Token expire time
    /// </summary>
    public DateTime RefreshTokenExpireTime { get; set; }
}