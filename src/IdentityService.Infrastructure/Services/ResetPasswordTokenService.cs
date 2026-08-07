using System.Security.Cryptography;
using IdentityService.Application.Interfaces;

namespace IdentityService.Infrastructure.Services;

public class ResetPasswordTokenService
    : IResetPasswordTokenService
{
    public string GenerateToken()
    {
        var bytes = new byte[64];

        using var rng =
            RandomNumberGenerator.Create();

        rng.GetBytes(bytes);

        return Convert.ToBase64String(bytes);
    }
}