namespace CleanAuth.API.Common;

public class EmailResult : Result
{
    public string? Message { get; }

    private EmailResult(bool isSuccess, string error, string? message = null) 
        : base(isSuccess, error)
    {
        Message = message;
    }

    public static EmailResult Success(string message) 
        => new(true, string.Empty, message);

    public new static EmailResult Failure(string error) 
        => new(false, error);
}