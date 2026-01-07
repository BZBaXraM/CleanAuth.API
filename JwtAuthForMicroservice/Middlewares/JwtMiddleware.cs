using JwtAuthForMicroservice.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace JwtAuthForMicroservice.Middlewares;

public partial class JwtMiddleware(IAccessTokenService accessTokenService, ILogger<JwtMiddleware> logger) : IMiddleware
{
    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        var authHeader = context.Request.Headers.Authorization.FirstOrDefault();
        if (!string.IsNullOrEmpty(authHeader) && authHeader.StartsWith("Bearer "))
        {
            var token = authHeader["Bearer ".Length..].Trim();

            LogAuthorizationHeaderAuthHeaderExtractedTokenToken(logger, authHeader, token);

            if (!string.IsNullOrWhiteSpace(token))
            {
                var tokenSegments = token.Split('.').Length;
                if (tokenSegments is 3 or 5)
                {
                    AttachUserToContext(context, token);
                }
                else
                {
                    LogInvalidTokenFormat(logger);
                }
            }
            else
            {
                LogTokenIsNullOrEmpty(logger);
            }
        }
        else
        {
            LogAuthorizationHeaderIsMissingOrNotInTheCorrectFormat(logger);
        }

        await next(context);
    }

    private void AttachUserToContext(HttpContext context, string token)
    {
        try
        {
            var principal = accessTokenService.GetPrincipalFromToken(token);
            if (principal != null)
            {
                context.User = principal;
                var name = context.User.Identity!.Name;
                if (name == null) return;
                LogUserAttachedToContextName(logger, name);
                Console.WriteLine($"User attached to context {context.User.Identity.Name}");
            }
            else
            {
                LogPrincipalIsNull(logger);
            }
        }
        catch (Exception ex)
        {
            LogErrorAttachingUserToContextExMessage(logger, ex.Message);
            Console.WriteLine($"Error attaching user to context {ex.Message}");
        }
    }

    [LoggerMessage(LogLevel.Information, "Authorization header: {authHeader}, Extracted token: {token}")]
    static partial void LogAuthorizationHeaderAuthHeaderExtractedTokenToken(ILogger<JwtMiddleware> logger,
        string authHeader, string token);

    [LoggerMessage(LogLevel.Warning, "Invalid token format")]
    static partial void LogInvalidTokenFormat(ILogger<JwtMiddleware> logger);

    [LoggerMessage(LogLevel.Warning, "Token is null or empty")]
    static partial void LogTokenIsNullOrEmpty(ILogger<JwtMiddleware> logger);

    [LoggerMessage(LogLevel.Warning, "Authorization header is missing or not in the correct format")]
    static partial void LogAuthorizationHeaderIsMissingOrNotInTheCorrectFormat(ILogger<JwtMiddleware> logger);

    [LoggerMessage(LogLevel.Information, "User attached to context {name}")]
    static partial void LogUserAttachedToContextName(ILogger<JwtMiddleware> logger, string name);

    [LoggerMessage(LogLevel.Warning, "Principal is null")]
    static partial void LogPrincipalIsNull(ILogger<JwtMiddleware> logger);

    [LoggerMessage(LogLevel.Error, "Error attaching user to context {exMessage}")]
    static partial void LogErrorAttachingUserToContextExMessage(ILogger<JwtMiddleware> logger, string exMessage);
}