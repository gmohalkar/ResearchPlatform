using MediatR;

namespace IdentityService.Application.Features.Authentication.ResetPassword;

public record ResetPasswordCommand(
    string Token,
    string NewPassword)
    : IRequest<bool>;