namespace CleanAuth.Application.Validators;

public class ConfirmEmailCodeDtoValidator : AbstractValidator<ConfirmEmailCodeRequest>
{
    public ConfirmEmailCodeDtoValidator()
    {
        RuleFor(x => x.Code)
            .NotEmpty()
            .WithMessage("Confirmation code is required.")
            .Length(6)
            .WithMessage("Confirmation code must be exactly 6 characters long.")
            .Matches("^[A-Z0-9]+$")
            .WithMessage("Confirmation code must contain only uppercase letters and numbers.");
    }
}
