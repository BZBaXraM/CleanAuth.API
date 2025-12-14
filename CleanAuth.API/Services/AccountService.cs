namespace CleanAuth.API.Services;

public class AccountService : IAccountService
{
    private readonly IUserRepository _userRepository;
    private readonly IJwtService _jwtService;
    private readonly IEmailService _emailService;
    private readonly IBlackListService _blackListService;
    private readonly RegisterRequestValidator _registerRequestValidator;
    private readonly LoginRequestValidator _loginRequestValidator;
    private readonly ILogger<AccountService> _logger;

    public AccountService(
        IUserRepository userRepository,
        IJwtService jwtService,
        IEmailService emailService,
        IBlackListService blackListService,
        RegisterRequestValidator registerRequestValidator,
        LoginRequestValidator loginRequestValidator,
        ILogger<AccountService> logger)
    {
        _userRepository = userRepository;
        _jwtService = jwtService;
        _emailService = emailService;
        _blackListService = blackListService;
        _registerRequestValidator = registerRequestValidator;
        _loginRequestValidator = loginRequestValidator;
        _logger = logger;
    }

    public async Task<AuthResult> RegisterUserAsync(RegisterRequest request)
    {
        try
        {
            // Валидация
            var validationResult = await _registerRequestValidator.ValidateAsync(request);
            if (!validationResult.IsValid)
            {
                var errors = string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage));
                return AuthResult.Failure($"Validation failed: {errors}");
            }

            // Проверка на существование email
            var existingUserByEmail = await _userRepository.GetUserByEmailAsync(request.Email);
            if (existingUserByEmail != null)
            {
                return AuthResult.Failure("Email already exists");
            }

            // Проверка на существование username
            var existingUserByUsername = await _userRepository.GetUserByUsernameAsync(request.Username);
            if (existingUserByUsername != null)
            {
                return AuthResult.Failure("Username already exists");
            }

            // Создание пользователя
            var user = new User
            {
                Email = request.Email.ToLower(),
                UserName = request.Username,
                DateOfBirth = request.DateOfBirth,
                Gender = request.Gender,
                IsEmailConfirmed = false,
                EmailConfirmationCode = GenerateRandomCode(),
                EmailConfirmationCodeExpireTime = DateTime.UtcNow.AddMinutes(5),
                Password = PasswordHash(request.Password)
            };

            await _userRepository.AddUserAsync(user);

            // Отправка кода подтверждения
            try
            {
                await _emailService.SendConfirmationEmailAsync(user.Email, user.EmailConfirmationCode);
                return AuthResult.Success("Registration successful. Confirmation code sent to your email.", user);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send confirmation email to {Email}", user.Email);
                return AuthResult.Success(
                    "Registration successful, but failed to send confirmation email. Please request a new code.", user);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during user registration");
            return AuthResult.Failure("Registration failed. Please try again.");
        }
    }

    public async Task<Result<LoginResponse>> LoginUserAsync(LoginRequest request)
    {
        try
        {
            // Валидация
            var validationResult = await _loginRequestValidator.ValidateAsync(request);
            if (!validationResult.IsValid)
            {
                var errors = string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage));
                return Result.Failure<LoginResponse>($"Validation failed: {errors}");
            }

            // Поиск пользователя
            var user = await _userRepository.GetUserByUsernameOrEmailAsync(request.UsernameOrEmail);

            if (user is not { IsEmailConfirmed: true })
            {
                return Result.Failure<LoginResponse>("Invalid username, email, or email not confirmed.");
            }

            // Проверка пароля
            if (!PasswordVerify(request.Password, user.Password))
            {
                return Result.Failure<LoginResponse>("Invalid password");
            }

            // Генерация refresh token
            user.RefreshToken = _jwtService.GenerateRefreshToken();
            user.RefreshTokenExpireTime = DateTime.UtcNow.AddDays(7);
            await _userRepository.SaveChangesAsync();

            var loginResponse = new LoginResponse(
                _jwtService.GenerateSecurityToken(user),
                user.RefreshToken,
                user.RefreshTokenExpireTime
            );

            return Result.Success(loginResponse);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during user login");
            return Result.Failure<LoginResponse>("Login failed. Please try again.");
        }
    }

    public async Task<EmailResult> ConfirmEmailAsync(string code)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(code))
            {
                return EmailResult.Failure("Confirmation code is required.");
            }

            var user = await _userRepository.GetUserByConfirmationCodeAsync(code);

            if (user == null)
            {
                return EmailResult.Failure("Invalid confirmation code.");
            }

            if (user.IsEmailConfirmed)
            {
                return EmailResult.Failure("Email is already confirmed.");
            }

            if (user.EmailConfirmationCodeExpireTime < DateTime.UtcNow)
            {
                return EmailResult.Failure("Confirmation code has expired. Please request a new code.");
            }

            user.IsEmailConfirmed = true;
            user.EmailConfirmationCode = null;
            user.EmailConfirmationCodeExpireTime = null;

            await _userRepository.SaveChangesAsync();

            return EmailResult.Success("Email confirmed successfully. You can now log in.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during email confirmation");
            return EmailResult.Failure("Email confirmation failed. Please try again.");
        }
    }

    public async Task<EmailResult> RequestConfirmationCodeAsync(string email)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                return EmailResult.Failure("Email is required.");
            }

            var user = await _userRepository.GetUserByEmailAsync(email.ToLower());

            if (user == null)
            {
                return EmailResult.Failure("User with this email not found.");
            }

            if (user.IsEmailConfirmed)
            {
                return EmailResult.Failure("Email is already confirmed.");
            }

            user.EmailConfirmationCode = GenerateRandomCode();
            user.EmailConfirmationCodeExpireTime = DateTime.UtcNow.AddMinutes(5);

            await _userRepository.SaveChangesAsync();

            try
            {
                await _emailService.SendConfirmationEmailAsync(user.Email, user.EmailConfirmationCode);
                return EmailResult.Success("Confirmation code sent to your email.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send confirmation email to {Email}", user.Email);
                return EmailResult.Failure("Failed to send confirmation email. Please try again later.");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during confirmation code request");
            return EmailResult.Failure("Failed to process request. Please try again.");
        }
    }

    public async Task<Result<TokenDto>> RefreshTokenAsync(string refreshToken)
    {
        try
        {
            var user = await _userRepository.GetUserByRefreshTokenAsync(refreshToken);

            if (user == null)
            {
                return Result.Failure<TokenDto>("Invalid refresh token");
            }

            if (user.RefreshTokenExpireTime < DateTime.UtcNow)
            {
                user.RefreshToken = null;
                user.RefreshTokenExpireTime = DateTime.UtcNow;
                await _userRepository.SaveChangesAsync();
                return Result.Failure<TokenDto>("Refresh token has expired");
            }

            user.RefreshToken = _jwtService.GenerateRefreshToken();
            user.RefreshTokenExpireTime = DateTime.UtcNow.AddDays(7);
            await _userRepository.SaveChangesAsync();

            var tokenDto = new TokenDto
            {
                Token = _jwtService.GenerateSecurityToken(user),
                RefreshToken = user.RefreshToken,
                RefreshTokenExpireTime = user.RefreshTokenExpireTime
            };

            return Result.Success(tokenDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during token refresh");
            return Result.Failure<TokenDto>("Token refresh failed. Please try again.");
        }
    }

    public async Task<Result> LogoutAsync(string accessToken, string? userName)
    {
        try
        {
            _blackListService.AddTokenToBlackList(accessToken);

            if (!string.IsNullOrEmpty(userName))
            {
                var user = await _userRepository.GetUserByUsernameAsync(userName);
                if (user != null)
                {
                    user.RefreshToken = null;
                    user.RefreshTokenExpireTime = DateTime.UtcNow;
                    await _userRepository.SaveChangesAsync();
                }
            }

            return Result.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during logout");
            return Result.Failure("Logout failed. Please try again.");
        }
    }

    private static string GenerateRandomCode(int length = 6)
    {
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        var random = new Random();

        return new string(Enumerable.Repeat(chars, length)
            .Select(s => s[random.Next(s.Length)]).ToArray());
    }

    private static string PasswordHash(string password)
    {
        return BCrypt.Net.BCrypt.HashPassword(password);
    }

    private static bool PasswordVerify(string password, string hash)
    {
        return BCrypt.Net.BCrypt.Verify(password, hash);
    }

    public async Task<Result<UserDto>> GetUserByIdAsync(Guid userId)
    {
        try
        {
            var user = await _userRepository.GetByIdAsync(userId);

            if (user == null)
            {
                return Result.Failure<UserDto>("User not found");
            }

            var userDto = new UserDto
            {
                Id = user.Id,
                Email = user.Email,
                UserName = user.UserName,
                DateOfBirth = user.DateOfBirth,
                Gender = user.Gender.ToString()
            };

            return Result.Success(userDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting user by ID {UserId}", userId);
            return Result.Failure<UserDto>("Failed to retrieve user information");
        }
    }
}