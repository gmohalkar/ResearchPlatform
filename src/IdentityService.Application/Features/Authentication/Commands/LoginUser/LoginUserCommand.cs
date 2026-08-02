using MediatR;

namespace IdentityService.Application.Features.Authentication.LoginUser;

public record LoginUserCommand(
    string Email,
    string Password)
    : IRequest<LoginResponse>;