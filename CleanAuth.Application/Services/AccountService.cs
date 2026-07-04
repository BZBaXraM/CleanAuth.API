using CleanAuth.Application.Extensions;

namespace CleanAuth.Application.Services;

public class AccountService : IAccountService
{
    private readonly IUnitOfWork _uow;
    private readonly IJwtService _jwtService;
    private readonly IEmailService _emailService;
    private readonly IBlackListService _blackListService;
    private readonly RegisterRequestValidator _registerRequestValidator;
    private readonly LoginRequestValidator _loginRequestValidator;
    private readonly ResetPasswordRequestValidator _resetPasswordRequestValidator;
    private readonly ChangePasswordRequestValidator _changePasswordRequestValidator;
    private readonly ChangeUsernameRequestValidator _changeUsernameRequestValidator;
    private readonly ILogger<AccountService> _logger;

    public AccountService(
        IUnitOfWork uow,
        IJwtService jwtService,
        IEmailService emailService,
        IBlackListService blackListService,
        RegisterRequestValidator registerRequestValidator,
        LoginRequestValidator loginRequestValidator,
        ResetPasswordRequestValidator resetPasswordRequestValidator,
        ChangePasswordRequestValidator changePasswordRequestValidator,
        ChangeUsernameRequestValidator changeUsernameRequestValidator,
        ILogger<AccountService> logger)
    {
        _uow = uow;
        _jwtService = jwtService;
        _emailService = emailService;
        _blackListService = blackListService;
        _registerRequestValidator = registerRequestValidator;
        _loginRequestValidator = loginRequestValidator;
        _resetPasswordRequestValidator = resetPasswordRequestValidator;
        _changePasswordRequestValidator = changePasswordRequestValidator;
        _changeUsernameRequestValidator = changeUsernameRequestValidator;
        _logger = logger;
    }

    public async Task<ResponseModel> RegisterUserAsync(RegisterRequest request)
    {
        var validationResult = await _registerRequestValidator.ValidateAsync(request);
        if (!validationResult.IsValid)
        {
            var errors = string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage));
            return ResponseModel.Failure($"Validation failed: {errors}");
        }

        if (await _uow.UserRepository.GetUserByEmailAsync(request.Email) != null)
            return ResponseModel.Failure("Email already exists");

        if (await _uow.UserRepository.GetUserByUsernameAsync(request.Username) != null)
            return ResponseModel.Failure("Username already exists");

        var user = new User
        {
            Email = request.Email.ToLower(),
            UserName = request.Username,
            Password = PasswordHelper.Hash(request.Password),
            Gender = request.Gender,
            DateOfBirth = request.DateOfBirth,
            IsEmailConfirmed = false,
            EmailConfirmationCode = CodeHelper.GenerateRandom(),
            EmailConfirmationCodeExpireTime = DateTime.UtcNow.AddMinutes(5),
        };

        _uow.UserRepository.AddUser(user);
        await _uow.CommitAsync();

        try
        {
            await _emailService.SendConfirmationEmailAsync(user.Email, user.EmailConfirmationCode);
            return ResponseModel.Success("Registration successful. Confirmation code sent to your email.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send confirmation email to {Email}", user.Email);
            return ResponseModel.Success(
                "Registration successful, but failed to send confirmation email. Please request a new code.");
        }
    }

    public async Task<ResponseModel<LoginResponse>> LoginUserAsync(LoginRequest request)
    {
        var validationResult = await _loginRequestValidator.ValidateAsync(request);
        if (!validationResult.IsValid)
        {
            var errors = string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage));
            return ResponseModel.Failure<LoginResponse>($"Validation failed: {errors}");
        }

        var user = await _uow.UserRepository.GetUserByUsernameOrEmailAsync(request.UsernameOrEmail);

        if (user is not { IsEmailConfirmed: true })
            return ResponseModel.Failure<LoginResponse>("Invalid username, email, or email not confirmed.");

        if (!PasswordHelper.Verify(request.Password, user.Password))
            return ResponseModel.Failure<LoginResponse>("Invalid password");

        user.RefreshToken = _jwtService.GenerateRefreshToken();
        user.RefreshTokenExpireTime = DateTime.UtcNow.AddDays(7);
        await _uow.CommitAsync();

        var loginResponse = new LoginResponse(
            _jwtService.GenerateSecurityToken(user, request.RememberMe),
            user.RefreshToken,
            user.RefreshTokenExpireTime
        );

        return ResponseModel.Success(loginResponse);
    }

    public async Task<ResponseModel> ConfirmEmailAsync(string code)
    {
        if (string.IsNullOrWhiteSpace(code))
            return ResponseModel.Failure("Confirmation code is required.");

        var user = await _uow.UserRepository.GetUserByConfirmationCodeAsync(code);

        if (user == null)
            return ResponseModel.Failure("Invalid confirmation code.");

        if (user.IsEmailConfirmed)
            return ResponseModel.Failure("Email is already confirmed.");

        if (user.EmailConfirmationCodeExpireTime < DateTime.UtcNow)
            return ResponseModel.Failure("Confirmation code has expired. Please request a new code.");

        user.IsEmailConfirmed = true;
        user.EmailConfirmationCode = null;
        user.EmailConfirmationCodeExpireTime = null;

        await _uow.CommitAsync();

        return ResponseModel.Success("Email confirmed successfully. You can now log in.");
    }

    public async Task<ResponseModel> RequestConfirmationCodeAsync(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
            return ResponseModel.Failure("Email is required.");

        var user = await _uow.UserRepository.GetUserByEmailAsync(email.ToLower());

        if (user == null)
            return ResponseModel.Failure("User with this email not found.");

        if (user.IsEmailConfirmed)
            return ResponseModel.Failure("Email is already confirmed.");

        user.EmailConfirmationCode = CodeHelper.GenerateRandom();
        user.EmailConfirmationCodeExpireTime = DateTime.UtcNow.AddMinutes(5);

        await _uow.CommitAsync();

        try
        {
            await _emailService.SendConfirmationEmailAsync(user.Email, user.EmailConfirmationCode);
            return ResponseModel.Success("Confirmation code sent to your email.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send confirmation email to {Email}", user.Email);
            return ResponseModel.Failure("Failed to send confirmation email. Please try again later.");
        }
    }

    public async Task<ResponseModel<TokenResponse>> RefreshTokenAsync(string refreshToken)
    {
        var user = await _uow.UserRepository.GetUserByRefreshTokenAsync(refreshToken);

        if (user == null)
            return ResponseModel.Failure<TokenResponse>("Invalid refresh token");

        if (user.RefreshTokenExpireTime < DateTime.UtcNow)
        {
            user.RefreshToken = null;
            user.RefreshTokenExpireTime = DateTime.UtcNow;
            await _uow.CommitAsync();
            return ResponseModel.Failure<TokenResponse>("Refresh token has expired");
        }

        user.RefreshToken = _jwtService.GenerateRefreshToken();
        user.RefreshTokenExpireTime = DateTime.UtcNow.AddDays(7);
        await _uow.CommitAsync();

        var tokenDto = new TokenResponse
        {
            Token = _jwtService.GenerateSecurityToken(user),
            RefreshToken = user.RefreshToken,
            RefreshTokenExpireTime = user.RefreshTokenExpireTime
        };

        return ResponseModel.Success(tokenDto);
    }

    public async Task<ResponseModel> LogoutAsync(string accessToken, string? userName)
    {
        _blackListService.AddTokenToBlackList(accessToken);

        if (!string.IsNullOrEmpty(userName))
        {
            var user = await _uow.UserRepository.GetUserByUsernameAsync(userName);
            if (user != null)
            {
                user.RefreshToken = null;
                user.RefreshTokenExpireTime = DateTime.UtcNow;
                await _uow.CommitAsync();
            }
        }

        return ResponseModel.Success();
    }

    public async Task<ResponseModel<UserResponse>> GetUserByIdAsync(Guid userId)
    {
        var user = await _uow.UserRepository.GetByIdAsync(userId);

        if (user == null)
            return ResponseModel.Failure<UserResponse>("User not found");

        var userDto = new UserResponse
        {
            Id = user.Id,
            Email = user.Email,
            UserName = user.UserName,
        };

        return ResponseModel.Success(userDto);
    }

    public async Task<ResponseModel> ForgetPasswordAsync(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
            return ResponseModel.Failure("Email is required.");

        var user = await _uow.UserRepository.GetUserByEmailAsync(email.ToLower());

        if (user == null)
            return ResponseModel.Failure("User with this email not found.");

        user.PasswordResetCode = CodeHelper.GenerateRandom();
        user.PasswordResetCodeExpireTime = DateTime.UtcNow.AddMinutes(15);

        await _uow.CommitAsync();

        try
        {
            await _emailService.SendPasswordResetEmailAsync(user.Email, user.PasswordResetCode);
            return ResponseModel.Success("Password reset code sent to your email.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send password reset email to {Email}", user.Email);
            return ResponseModel.Failure("Failed to send password reset email. Please try again later.");
        }
    }

    public async Task<ResponseModel> ResetPasswordAsync(ResetPasswordRequest request)
    {
        var validationResult = await _resetPasswordRequestValidator.ValidateAsync(request);
        if (!validationResult.IsValid)
        {
            var errors = string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage));
            return ResponseModel.Failure($"Validation failed: {errors}");
        }

        var user = await _uow.UserRepository.GetUserByPasswordResetCodeAsync(request.Code);

        if (user == null)
            return ResponseModel.Failure("Invalid reset code.");

        if (!string.Equals(user.Email, request.Email, StringComparison.CurrentCultureIgnoreCase))
            return ResponseModel.Failure("Email does not match the reset code.");

        if (user.PasswordResetCodeExpireTime == null || user.PasswordResetCodeExpireTime < DateTime.UtcNow)
            return ResponseModel.Failure("Reset code has expired. Please request a new code.");

        user.Password = PasswordHelper.Hash(request.NewPassword);
        user.PasswordResetCode = null;
        user.PasswordResetCodeExpireTime = null;
        user.RefreshToken = null;
        user.RefreshTokenExpireTime = DateTime.UtcNow;

        await _uow.CommitAsync();

        return ResponseModel.Success("Password reset successfully. You can now log in with your new password.");
    }

    public async Task<ResponseModel> ChangePasswordAsync(Guid userId, ChangePasswordRequest request)
    {
        var validationResult = await _changePasswordRequestValidator.ValidateAsync(request);
        if (!validationResult.IsValid)
        {
            var errors = string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage));
            return ResponseModel.Failure($"Validation failed: {errors}");
        }

        var user = await _uow.UserRepository.GetByIdAsync(userId);

        if (user == null)
            return ResponseModel.Failure("User not found");

        if (!PasswordHelper.Verify(request.CurrentPassword, user.Password))
            return ResponseModel.Failure("Current password is incorrect");

        user.Password = PasswordHelper.Hash(request.NewPassword);
        user.RefreshToken = null;
        user.RefreshTokenExpireTime = DateTime.UtcNow;

        await _uow.CommitAsync();

        return ResponseModel.Success("Password changed successfully. Please log in again.");
    }

    public async Task<ResponseModel<UserResponse>> ChangeUsernameAsync(Guid userId, ChangeUsernameRequest request)
    {
        var validationResult = await _changeUsernameRequestValidator.ValidateAsync(request);
        if (!validationResult.IsValid)
        {
            var errors = string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage));
            return ResponseModel.Failure<UserResponse>($"Validation failed: {errors}");
        }

        var user = await _uow.UserRepository.GetByIdAsync(userId);

        if (user == null)
            return ResponseModel.Failure<UserResponse>("User not found");

        if (string.Equals(user.UserName, request.NewUsername, StringComparison.Ordinal))
            return ResponseModel.Failure<UserResponse>("New username must be different from the current username");

        var existing = await _uow.UserRepository.GetUserByUsernameAsync(request.NewUsername);
        if (existing != null && existing.Id != user.Id)
            return ResponseModel.Failure<UserResponse>("Username already exists");

        user.UserName = request.NewUsername;

        await _uow.CommitAsync();

        return ResponseModel.Success(user.ToUserDto());
    }
}