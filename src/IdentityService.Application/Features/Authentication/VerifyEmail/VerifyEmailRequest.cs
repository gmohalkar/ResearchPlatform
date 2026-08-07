namespace IdentityService.Application.Features.Authentication.VerifyEmail;

public class VerifyEmailRequest
{
    public string Token
    {
        get;
        set;
    } = string.Empty;
}