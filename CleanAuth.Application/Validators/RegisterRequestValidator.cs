namespace CleanAuth.Application.Validators;

public class RegisterRequestValidator : AbstractValidator<RegisterRequest>
{
    public RegisterRequestValidator()
    {
        RuleFor(x => x.Username)
            .Username()
            .WithMessage("Username must be 3-20 characters long and contain only letters, numbers, and underscores.");

        RuleFor(x => x.Email)
            .Email()
            .WithMessage("Please provide a valid email address.");

        RuleFor(x => x.Password)
            .NotEmpty()
            .MinimumLength(8)
            .MaximumLength(30)
            .Matches(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)")
            .WithMessage(
                "Password must be 8-30 characters long and contain at least one lowercase letter, one uppercase letter, and one digit.");
    }
}
