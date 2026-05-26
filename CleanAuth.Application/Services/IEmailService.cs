namespace CleanAuth.Application.Services;

public interface IEmailService
{
    /// <summary>
    /// Send email
    /// </summary>
    Task SendEmailAsync(string email, string subject, string message);

    Task SendConfirmationEmailAsync(string email, string code);

    Task SendPasswordResetEmailAsync(string email, string code);
}
