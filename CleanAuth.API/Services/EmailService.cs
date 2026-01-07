namespace CleanAuth.API.Services;

public class EmailService : IEmailService
{
    private const string Subject = "CleanAuth - Email Confirmation";
    private readonly SmtpClient _client;
    private readonly EmailConfig _config;
    private readonly ILogger<EmailService> _logger;

    public EmailService(EmailConfig config, ILogger<EmailService> logger, SmtpClient client)
    {
        _config = config;
        _logger = logger;
        _client = client;
    }

    public async Task SendEmailAsync(string email, string subject, string message)
    {
        try
        {
            MimeMessage emailMessage = new();

            emailMessage.From.Add(new MailboxAddress("CleanAuth", _config.From));
            emailMessage.To.Add(new MailboxAddress("User", email));
            emailMessage.Subject = subject;
            emailMessage.Body = new TextPart(TextFormat.Html)
            {
                Text = message
            };

            _client.ServerCertificateValidationCallback = (s, c, h, e) => true;

            await _client.ConnectAsync(_config.SmtpServer, _config.Port, SecureSocketOptions.StartTls);
            await _client.AuthenticateAsync(_config.UserName, _config.Password);
            await _client.SendAsync(emailMessage);

            await _client.DisconnectAsync(true);

            _logger.LogInformation("Email sent successfully to {Email}", email);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send email to {Email}", email);
            throw;
        }
    }

    public async Task SendAsync(MimeMessage message)
    {
        try
        {
            _client.ServerCertificateValidationCallback = (s, c, h, e) => true;

            await _client.ConnectAsync(_config.SmtpServer, _config.Port, SecureSocketOptions.StartTls);
            await _client.AuthenticateAsync(_config.UserName, _config.Password);
            await _client.SendAsync(message);

            await _client.DisconnectAsync(true);

            _logger.LogInformation("Email message sent successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send email message");
            throw;
        }
    }

    public async Task SendConfirmationEmailAsync(string email, string code)
    {
        var message = $"""

                                   <html>
                                   <body>
                                       <h2>Welcome!</h2>
                                       <p>Thank you for registering. Please use the following confirmation code to verify your email address:</p>
                                       <h3 style='color: #007bff; font-size: 24px; letter-spacing: 2px;'>{code}</h3>
                                       <p>This code will expire in 5 minutes.</p>
                                       <p>If you didn't create an account with CleanAuth, please ignore this email.</p>
                                   </body>
                                   </html>
                       """;

        await SendEmailAsync(email, Subject, message);
    }
}