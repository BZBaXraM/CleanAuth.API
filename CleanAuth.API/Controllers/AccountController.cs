namespace CleanAuth.API.Controllers;

/// <summary>
/// AccountController
/// </summary>
[Route("api/[controller]")]
[ApiController]
public class AccountController : ControllerBase
{
    private readonly IAccountService _accountService;
    private readonly ICurrentUserService _currentUser;

    /// <inheritdoc />
    public AccountController(IAccountService accountService, ICurrentUserService currentUser)
    {
        _accountService = accountService;
        _currentUser = currentUser;
    }

    /// <summary>
    /// Register a new user
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    [HttpPost("register")]
    public async Task<ActionResult<ResponseModel<RegisterResponse>>> RegisterAsync([FromBody] RegisterRequest request)
    {
        var result = await _accountService.RegisterUserAsync(request);

        return StatusCode(result.StatusCode, result);
    }

    /// <summary>
    /// Log in a user
    /// </summary>
    [HttpPost("login")]
    public async Task<ActionResult<ResponseModel<LoginResponse>>> LoginAsync([FromBody] LoginRequest request)
    {
        var result = await _accountService.LoginUserAsync(request);

        return StatusCode(result.StatusCode, result);
    }

    /// <summary>
    /// Refresh the token of a user
    /// </summary>
    [HttpPost("refresh-token")]
    public async Task<ActionResult<ResponseModel<TokenResponse>>> RefreshTokenAsync([FromBody] RefreshTokenRequest tokenRequest)
    {
        var result = await _accountService.RefreshTokenAsync(tokenRequest.RefreshToken);

        return StatusCode(result.StatusCode, result);
    }

    /// <summary>
    /// Logout a user
    /// </summary>
    [HttpPost("logout")]
    [Authorize]
    public async Task<ActionResult<ResponseModel>> LogoutAsync([FromBody] TokenResponse response)
    {
        var result = await _accountService.LogoutAsync(response.Token, User.Identity?.Name);

        return StatusCode(result.StatusCode, result);
    }

    /// <summary>
    /// Confirm the email of a user using a confirmation code
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    [HttpPost("confirm-email-code")]
    public async Task<IActionResult> ConfirmEmailCodeAsync([FromBody] ConfirmEmailCodeRequest request)
    {
        var result = await _accountService.ConfirmEmailAsync(request.Code);

        return StatusCode(result.StatusCode, result);
    }

    /// <summary>
    /// Request a new confirmation code to be sent to the user's email
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    [HttpPost("request-confirmation-code")]
    public async Task<ActionResult<ResponseModel>> RequestConfirmationCode([FromBody] RequestConfirmationCodeRequest request)
    {
        var result = await _accountService.RequestConfirmationCodeAsync(request.Email);

        return StatusCode(result.StatusCode, result);
    }

    /// <summary>
    /// Get current user information
    /// </summary>
    [HttpGet("me")]
    [Authorize]
    public async Task<ActionResult<ResponseModel<UserResponse>>> GetCurrentUser()
    {
        if (_currentUser.UserId is not { } id)
        {
            var failure = ResponseModel.Failure<ResponseModel>("Invalid user token");
            return StatusCode(failure.StatusCode, failure);
        }

        var result = await _accountService.GetUserByIdAsync(id);
        return StatusCode(result.StatusCode, result);
    }

    /// <summary>
    /// Request password reset code
    /// </summary>
    [HttpPost("forget-password")]
    public async Task<ActionResult<ResponseModel>> ForgetPasswordAsync([FromBody] ForgetPasswordRequest request)
    {
        var result = await _accountService.ForgetPasswordAsync(request.Email);

        return StatusCode(result.StatusCode, result);
    }

    /// <summary>
    /// Reset password using code
    /// </summary>
    [HttpPost("reset-password")]
    public async Task<ActionResult<ResponseModel>> ResetPasswordAsync([FromBody] ResetPasswordRequest request)
    {
        var result = await _accountService.ResetPasswordAsync(request);

        return StatusCode(result.StatusCode, result);
    }

    /// <summary>
    /// Change the password of the currently authenticated user
    /// </summary>
    [HttpPost("change-password")]
    [Authorize]
    public async Task<ActionResult<ResponseModel>> ChangePasswordAsync([FromBody] ChangePasswordRequest request)
    {
        if (_currentUser.UserId is not { } id)
        {
            var failure = ResponseModel.Failure("Invalid user token");
            return StatusCode(failure.StatusCode, failure);
        }

        var result = await _accountService.ChangePasswordAsync(id, request);

        return StatusCode(result.StatusCode, result);
    }

    /// <summary>
    /// Change the username of the currently authenticated user
    /// </summary>
    [HttpPost("change-username")]
    [Authorize]
    public async Task<ActionResult<ResponseModel<UserResponse>>> ChangeUsernameAsync([FromBody] ChangeUsernameRequest request)
    {
        if (_currentUser.UserId is not { } id)
        {
            var failure = ResponseModel.Failure<UserResponse>("Invalid user token");
            return StatusCode(failure.StatusCode, failure);
        }

        var result = await _accountService.ChangeUsernameAsync(id, request);

        return StatusCode(result.StatusCode, result);
    }
}