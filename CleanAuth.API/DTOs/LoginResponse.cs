namespace CleanAuth.API.DTOs;

public class LoginResponse
{
    public string Token { get; set; } = string.Empty;
    public string RefreshToken { get; set; } = string.Empty;
    public DateTime? RefreshTokenExpireTime { get; set; }

    public LoginResponse(string token, string refreshToken, DateTime? refreshTokenExpireTime)
    {
        Token = token;
        RefreshToken = refreshToken;
        RefreshTokenExpireTime = refreshTokenExpireTime;
    }
}