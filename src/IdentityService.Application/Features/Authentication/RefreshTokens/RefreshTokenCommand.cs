using MediatR;

namespace IdentityService.Application.Features.Authentication.RefreshToken;
public record RefreshTokenCommand(
    string RefreshToken)
    : IRequest<RefreshTokenResponse>;
