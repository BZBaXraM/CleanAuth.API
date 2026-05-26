namespace CleanAuth.Application.DTOs;

/// <summary>
/// Request model for refreshing JWT tokens
/// </summary>
public class RefreshTokenRequest
{
    /// <summary>
    /// Refresh token to obtain a new access token
    /// </summary>
    public string RefreshToken { get; set; } = null!;
}
