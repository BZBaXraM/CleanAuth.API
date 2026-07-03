namespace CleanAuth.Application.Validators;

public class ChangeUsernameRequestValidator : AbstractValidator<ChangeUsernameRequest>
{
    public ChangeUsernameRequestValidator()
    {
        RuleFor(x => x.NewUsername)
            .Username()
            .WithMessage("Username must be 3-20 characters long and contain only letters, numbers, and underscores.");
    }
}
