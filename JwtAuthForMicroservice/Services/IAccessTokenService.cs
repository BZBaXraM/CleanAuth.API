using System.Collections.Concurrent;
using System.Security.Claims;

namespace JwtAuthForMicroservice.Services;

public interface IAccessTokenService
{
    ClaimsPrincipal? GetPrincipalFromToken(string token);
}