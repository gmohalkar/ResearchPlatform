using FluentValidation;

namespace IdentityService.Application.Features.Authentication.LoginUser;

public class LoginUserValidator
    : AbstractValidator<LoginUserCommand>
{
    public LoginUserValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty()
            .EmailAddress();

        RuleFor(x => x.Password)
            .NotEmpty();
    }
}