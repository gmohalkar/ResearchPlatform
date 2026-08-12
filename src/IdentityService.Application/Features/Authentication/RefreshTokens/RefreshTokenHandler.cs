using System.Security;
using IdentityService.Application.Features.Authentication.RefreshToken;
using IdentityService.Application.Interfaces;
using IdentityService.Domain.Entities;
using MediatR;

public class RefreshTokenHandler
    : IRequestHandler<
        RefreshTokenCommand,
        RefreshTokenResponse>
{
    private readonly IRefreshTokenRepository _refreshTokenRepository;
    private readonly IJwtService _jwtService;
    private readonly IRefreshTokenService _refreshTokenService;

    private readonly IPermissionRepository _permissionRepository;

    public RefreshTokenHandler(
        IRefreshTokenRepository refreshTokenRepository,
        IJwtService jwtService,
        IRefreshTokenService refreshTokenService,
        IPermissionRepository permissionRepository)
    {
        _refreshTokenRepository = refreshTokenRepository;
        _jwtService = jwtService;
        _refreshTokenService = refreshTokenService;
        _permissionRepository = permissionRepository;
    }
   public async Task<RefreshTokenResponse> Handle(
    RefreshTokenCommand request,
    CancellationToken cancellationToken)
{
    var existingRefreshToken =
        await _refreshTokenRepository
            .GetByTokenAsync(
                request.RefreshToken);

    if (existingRefreshToken == null)
    {
        throw new Exception(
            "Refresh token not found.");
    }

    if (existingRefreshToken.IsRevoked)
    {
        throw new Exception(
            "Refresh token revoked.");
    }

    if (existingRefreshToken.Expires < DateTime.UtcNow)
    {
        throw new Exception(
            "Refresh token expired.");
    }

    var user = existingRefreshToken.User;
    if (user == null)
    {
        throw new Exception(
            "User associated with refresh token not found.");
    }

    var permissions =
    await _permissionRepository
        .GetPermissionsByRoleIdAsync(
            user.RoleId);
    var newAccessToken =
        _jwtService.GenerateToken(user, permissions);

    var newRefreshToken =
        _refreshTokenService
            .GenerateRefreshToken();

    existingRefreshToken.IsRevoked = true;

    await _refreshTokenRepository
        .UpdateAsync(existingRefreshToken);

    await _refreshTokenRepository.AddAsync(
        new RefreshToken
        {
            Token = newRefreshToken,
            UserId = user.Id,
            Expires = DateTime.UtcNow.AddDays(7),
            IsRevoked = false
        });

    await _refreshTokenRepository
        .SaveChangesAsync();

    return new RefreshTokenResponse
    {
        AccessToken = newAccessToken,
        RefreshToken = newRefreshToken,
        ExpiresAt =
            DateTime.UtcNow.AddMinutes(60)
    };
}
}