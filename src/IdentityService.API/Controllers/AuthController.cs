using IdentityService.Application.Features.Authentication.LoginUser;
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
}
