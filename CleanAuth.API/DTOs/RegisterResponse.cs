namespace CleanAuth.API.DTOs;

public class RegisterResponse
{
    public bool Success { get; set; }
    public string Message { get; set; }
    public string? Email { get; set; }

    public RegisterResponse(bool success, string message, string? email = null)
    {
        Success = success;
        Message = message;
        Email = email;
    }
}