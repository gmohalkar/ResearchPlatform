using IdentityService.Application.Interfaces;
using MediatR;

namespace IdentityService.Application.Features.Authentication.ResetPassword;

public class ResetPasswordHandler
    : IRequestHandler<
        ResetPasswordCommand,
        bool>
{
    private readonly IUserRepository
        _userRepository;

    private readonly IPasswordService
        _passwordService;

    private readonly IAuditService _auditService;

    public ResetPasswordHandler(
        IUserRepository userRepository,
        IPasswordService passwordService,
        IAuditService auditService)
    {
        _userRepository = userRepository;
        _passwordService = passwordService;
        _auditService = auditService;
    }

    public async Task<bool> Handle(
        ResetPasswordCommand request,
        CancellationToken cancellationToken)
    {
        var user =
            await _userRepository
                .GetByResetTokenAsync(
                    request.Token);

        if (user == null)
        {
            throw new Exception(
                "Invalid reset token.");
        }

        if (
            user.PasswordResetTokenExpiry
            < DateTime.UtcNow)
        {
            throw new Exception(
                "Reset token expired.");
        }

        user.PasswordHash =
            _passwordService.HashPassword(
                request.NewPassword);

        user.PasswordResetToken = null;

        user.PasswordResetTokenExpiry = null;

        await _userRepository.UpdateAsync(user);

        await _userRepository.SaveChangesAsync();

        await _auditService.LogAsync(
            user.Id,
            "ResetPassword",
            "/api/auth/reset-password",
            "User reset their password");

        return true;
    }
}