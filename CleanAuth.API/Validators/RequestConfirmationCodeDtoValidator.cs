namespace CleanAuth.API.Validators;

public class RequestConfirmationCodeDtoValidator : AbstractValidator<RequestConfirmationCodeDto>
{
    public RequestConfirmationCodeDtoValidator()
    {
        RuleFor(x => x.Email)
            .Email()
            .WithMessage("Please provide a valid email address.");
    }
}