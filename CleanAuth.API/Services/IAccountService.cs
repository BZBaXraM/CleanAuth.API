namespace CleanAuth.API.Services;

public interface IAccountService
{
    Task<AuthResult> RegisterUserAsync(RegisterRequest request);
    Task<Result<LoginResponse>> LoginUserAsync(LoginRequest request);
    Task<EmailResult> ConfirmEmailAsync(string code);
    Task<EmailResult> RequestConfirmationCodeAsync(string email);
    Task<Result<TokenDto>> RefreshTokenAsync(string refreshToken);
    Task<Result> LogoutAsync(string accessToken, string? userName);
    Task<Result<UserDto>> GetUserByIdAsync(Guid userId);
}