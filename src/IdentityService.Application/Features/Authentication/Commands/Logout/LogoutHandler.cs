using IdentityService.Application.Interfaces;
using MediatR;

namespace IdentityService.Application.Features.Authentication.Logout;

public class LogoutHandler
    : IRequestHandler<LogoutCommand, bool>
{
    private readonly IRefreshTokenRepository
        _refreshTokenRepository;
    private readonly IAuditService _auditService;

    public LogoutHandler(
        IRefreshTokenRepository refreshTokenRepository,
        IAuditService auditService)
    {
        _refreshTokenRepository =
            refreshTokenRepository;
        _auditService = auditService;
    }

    public async Task<bool> Handle(
        LogoutCommand request,
        CancellationToken cancellationToken)
    {
        var refreshToken =
            await _refreshTokenRepository
                .GetByTokenAsync(
                    request.RefreshToken);

        if (refreshToken == null)
        {
            throw new Exception(
                "Refresh token not found.");
        }

        refreshToken.IsRevoked = true;

        await _refreshTokenRepository
            .UpdateAsync(refreshToken);

        await _refreshTokenRepository
            .SaveChangesAsync();
        await _auditService.LogAsync(
            refreshToken.UserId,
            "Logout",
            "/api/auth/logout",
            "User logged out");
        return true;
    }
}