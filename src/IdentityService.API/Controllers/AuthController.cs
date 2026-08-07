using IdentityService.Application.Features.Authentication.ForgotPassword;
using IdentityService.Application.Features.Authentication.LoginUser;
using IdentityService.Application.Features.Authentication.Logout;
using IdentityService.Application.Features.Authentication.RefreshToken;
using IdentityService.Application.Features.Authentication.ResetPassword;
using IdentityService.Application.Features.Authentication.VerifyEmail;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IMediator _mediator;
public AuthController(IMediator mediator)
{
_mediator = mediator;
}

[HttpPost("register")]
public async Task<IActionResult> Register(
    RegisterUserCommand command)
{
    var result =
        await _mediator.Send(command);

    return Ok(result);
}

[HttpPost("login")]
public async Task<IActionResult> Login(
LoginUserCommand command)
{
var result =
await _mediator.Send(command);
return Ok(result);
}
[HttpPost("refresh")]
public async Task<IActionResult> Refresh(
    RefreshTokenRequest request)
{
    var result =
        await _mediator.Send(
            new RefreshTokenCommand(
                request.RefreshToken));

    return Ok(result);
}
[HttpPost("logout")]
public async Task<IActionResult> Logout(
    IdentityService.Application.Features.Authentication.Logout.LogoutRequest request)
{
    var result =
        await _mediator.Send(
            new IdentityService.Application.Features.Authentication.Logout.LogoutCommand(
                request.RefreshToken));

    return Ok(new
    {
        Message = "Logged out successfully"
    });
}
[HttpPost("forgot-password")]
public async Task<IActionResult> ForgotPassword(
    ForgotPasswordRequest request)
{
    await _mediator.Send(
        new ForgotPasswordCommand(
            request.Email));

    return Ok(new
    {
        Message =
            "If an account exists, a reset link has been sent."
    });
}
[HttpPost("reset-password")]
public async Task<IActionResult> ResetPassword(
    ResetPasswordRequest request)
{
    await _mediator.Send(
        new ResetPasswordCommand(
            request.Token,
            request.NewPassword));

    return Ok(new
    {
        Message =
            "Password reset successfully."
    });
}
[HttpPost("verify-email")]
public async Task<IActionResult> VerifyEmail(
    VerifyEmailRequest request)
{
    await _mediator.Send(
        new VerifyEmailCommand(
            request.Token));

    return Ok(new
    {
        Message =
            "Email verified successfully."
    });
}
}
