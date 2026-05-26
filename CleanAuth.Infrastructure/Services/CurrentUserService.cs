namespace CleanAuth.Infrastructure.Services;

public class CurrentUserService : ICurrentUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentUserService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public Guid? UserId
    {
        get
        {
            var claims = _httpContextAccessor.HttpContext?.User;
            var value = claims?.FindFirst(ClaimTypes.NameIdentifier)?.Value
                        ?? claims?.FindFirst(JwtRegisteredClaimNames.NameId)?.Value;

            if (Guid.TryParse(value, out var userId))
            {
                return userId;
            }

            return null;
        }
    }
}