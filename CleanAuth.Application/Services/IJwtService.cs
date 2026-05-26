namespace CleanAuth.Application.Services;

public interface IJwtService
{
    /// <summary>
    /// Generate a security token
    /// </summary>
    string GenerateSecurityToken(User user);

    /// <summary>
    /// Generate an email confirmation token
    /// </summary>
    string GenerateEmailConfirmationToken(User user);

    /// <summary>
    /// Validate an email confirmation token
    /// </summary>
    bool ValidateEmailConfirmationToken(User user, string token);

    /// <summary>
    /// Get the principal from a token
    /// </summary>
    ClaimsPrincipal GetPrincipalFromToken(string token);

    /// <summary>
    /// Generate a refresh token
    /// </summary>
    string GenerateRefreshToken();

    /// <summary>
    /// Generate a refresh token for email
    /// </summary>
    string GenerateRefreshTokenForEmail(User user);
}
