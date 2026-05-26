namespace CleanAuth.Application.Validators;

public class RequestConfirmationCodeDtoValidator : AbstractValidator<RequestConfirmationCodeRequest>
{
    public RequestConfirmationCodeDtoValidator()
    {
        RuleFor(x => x.Email)
            .Email()
            .WithMessage("Please provide a valid email address.");
    }
}
