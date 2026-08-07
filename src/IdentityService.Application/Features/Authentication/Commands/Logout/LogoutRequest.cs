namespace IdentityService.Application.Features.Authentication.Logout;

public class LogoutRequest
{
    public string RefreshToken
    {
        get;
        set;
    } = string.Empty;
}