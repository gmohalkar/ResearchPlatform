using FluentValidation;

public class RegisterUserValidator
    : AbstractValidator<RegisterUserCommand>
{
    public RegisterUserValidator()
    {
        RuleFor(x => x.FirstName)
            .NotEmpty();

        RuleFor(x => x.Email)
            .EmailAddress();

        RuleFor(x => x.Password)
            .MinimumLength(8);
        RuleFor(x => x.RoleName)
    .NotEmpty()
    .WithMessage("Role is required.");
    }
}