using System.Security.Cryptography;
using IdentityService.Application.Interfaces;

namespace IdentityService.Infrastructure.Services;

public class RefreshTokenService
    : IRefreshTokenService
{
    public string GenerateRefreshToken()
    {
        var randomBytes = new byte[64];

        using var rng =
            RandomNumberGenerator.Create();

        rng.GetBytes(randomBytes);

        return Convert.ToBase64String(
            randomBytes);
    }
}