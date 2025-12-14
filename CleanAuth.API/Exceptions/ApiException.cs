namespace CleanAuth.API.Exceptions;

public class ApiException(int statusCode, string message, string? details)
{
    public int StatusCode { get; set; } = statusCode;
    public string Message { get; set; } = message;
    public string? Details { get; set; } = details;
}

public class LoginException(string message, string? details = null)
    : ApiException((int)HttpStatusCode.Unauthorized, message, details);

public class RegisterException(string message, string? details = null)
    : ApiException((int)HttpStatusCode.BadRequest, message, details);