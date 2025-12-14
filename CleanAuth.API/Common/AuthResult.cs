namespace CleanAuth.API.Common;

public class AuthResult : Result
{
    public User? User { get; }
    public string? Message { get; }

    private AuthResult(bool isSuccess, string error, User? user = null, string? message = null) 
        : base(isSuccess, error)
    {
        User = user;
        Message = message;
    }

    public static AuthResult Success(string message, User? user = null) 
        => new(true, string.Empty, user, message);

    public new static AuthResult Failure(string error) 
        => new(false, error);
}