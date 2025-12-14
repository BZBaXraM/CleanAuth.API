namespace CleanAuth.API.Middlewares;

public class BlackListMiddleware : IMiddleware
{
    private readonly IBlackListService _blackListService;

    public BlackListMiddleware(IBlackListService blackListService)
    {
        _blackListService = blackListService;
    }

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        string? token = context.Request.Headers.Authorization;

        token = token?.Replace("Bearer ", "");

        if (string.IsNullOrWhiteSpace(token))
        {
            await next(context);
            return;
        }

        if (_blackListService.IsTokenBlackListed(token))
        {
            context.Response.StatusCode = 401;
            await context.Response.WriteAsync("Unauthorized");
            return;
        }

        await next(context);
    }
}