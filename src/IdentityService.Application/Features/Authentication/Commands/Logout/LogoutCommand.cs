using MediatR;

namespace IdentityService.Application.Features.Authentication.Logout;

public record LogoutCommand(
    string RefreshToken)
    : IRequest<bool>;