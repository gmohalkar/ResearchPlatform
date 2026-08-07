namespace IdentityService.Application.Features.Authentication.ForgotPassword;

public class ForgotPasswordRequest
{
    public string Email
    {
        get;
        set;
    } = string.Empty;
}