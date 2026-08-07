using MediatR;

namespace IdentityService.Application.Features.Authentication.VerifyEmail;

public record VerifyEmailCommand(
    string Token)
    : IRequest<bool>;