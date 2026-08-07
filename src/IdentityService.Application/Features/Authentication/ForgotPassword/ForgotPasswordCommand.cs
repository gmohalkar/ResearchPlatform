using MediatR;

namespace IdentityService.Application.Features.Authentication.ForgotPassword;

public record ForgotPasswordCommand(
    string Email)
    : IRequest<bool>;