namespace CleanAuth.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AccountController : ControllerBase
{
    private readonly IAccountService _accountService;

    public AccountController(IAccountService accountService)
    {
        _accountService = accountService;
    }

    /// <summary>
    /// Register a new user
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    [HttpPost("register")]
    public async Task<ActionResult<RegisterResponse>> RegisterAsync([FromBody] RegisterRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var result = await _accountService.RegisterUserAsync(request);

        if (result.IsFailure)
        {
            return BadRequest(new RegisterException(result.Error));
        }

        return Ok(new RegisterResponse(true, result.Message!, result.User?.Email));
    }

    /// <summary>
    /// Login a user
    /// </summary>
    [HttpPost("login")]
    public async Task<ActionResult<LoginResponse>> LoginAsync([FromBody] LoginRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var result = await _accountService.LoginUserAsync(request);

        if (result.IsFailure)
        {
            return Unauthorized(new LoginException(result.Error));
        }

        return Ok(result.Value);
    }

    /// <summary>
    /// Refresh the token of a user
    /// </summary>
    [HttpPost("refresh-token")]
    public async Task<ActionResult<TokenDto>> RefreshTokenAsync([FromBody] RefreshTokenDto tokenDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var result = await _accountService.RefreshTokenAsync(tokenDto.RefreshToken);

        if (result.IsFailure)
        {
            return BadRequest(result.Error);
        }

        return Ok(result.Value);
    }

    /// <summary>
    /// Logout a user
    /// </summary>
    [HttpPost("logout")]
    [Authorize]
    public async Task<IActionResult> LogoutAsync([FromBody] TokenDto dto)
    {
        var result = await _accountService.LogoutAsync(dto.Token, User.Identity?.Name);

        if (result.IsFailure)
        {
            return BadRequest(result.Error);
        }

        return Ok("Logged out successfully");
    }

    /// <summary>
    /// Confirm the email of a user using a confirmation code
    /// </summary>
    /// <param name="dto"></param>
    /// <returns></returns>
    [HttpPost("confirm-email-code")]
    public async Task<IActionResult> ConfirmEmailCodeAsync([FromBody] ConfirmEmailCodeDto dto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var result = await _accountService.ConfirmEmailAsync(dto.Code);

        if (result.IsFailure)
        {
            return BadRequest(result.Error);
        }

        return Ok(result.Message);
    }

    [HttpPost("request-confirmation-code")]
    public async Task<IActionResult> RequestConfirmationCode([FromBody] RequestConfirmationCodeDto dto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var result = await _accountService.RequestConfirmationCodeAsync(dto.Email);

        if (result.IsFailure)
        {
            return BadRequest(result.Error);
        }

        return Ok(result.Message);
    }

    /// <summary>
    /// Get current user information
    /// </summary>
    [HttpGet("me")]
    [Authorize]
    public async Task<ActionResult<UserDto>> GetCurrentUser()
    {
        var user = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrEmpty(user) || !Guid.TryParse(user, out var id))
        {
            return Unauthorized("Invalid user token");
        }

        var result = await _accountService.GetUserByIdAsync(id);

        if (result.IsFailure)
        {
            return NotFound(result.Error);
        }

        return Ok(result.Value);
    }
}