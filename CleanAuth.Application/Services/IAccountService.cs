namespace CleanAuth.Application.Services;

public interface IAccountService
{
    Task<ResponseModel> RegisterUserAsync(RegisterRequest request);
    Task<ResponseModel<LoginResponse>> LoginUserAsync(LoginRequest request);
    Task<ResponseModel> ConfirmEmailAsync(string code);
    Task<ResponseModel> RequestConfirmationCodeAsync(string email);
    Task<ResponseModel<TokenResponse>> RefreshTokenAsync(string refreshToken);
    Task<ResponseModel> LogoutAsync(string accessToken, string? userName);
    Task<ResponseModel<UserResponse>> GetUserByIdAsync(Guid userId);
    Task<ResponseModel> ForgetPasswordAsync(string email);
    Task<ResponseModel> ResetPasswordAsync(ResetPasswordRequest request);
    Task<ResponseModel> ChangePasswordAsync(Guid userId, ChangePasswordRequest request);
    Task<ResponseModel<UserResponse>> ChangeUsernameAsync(Guid userId, ChangeUsernameRequest request);
}
