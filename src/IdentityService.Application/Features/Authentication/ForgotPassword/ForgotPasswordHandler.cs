using System.Security.Cryptography;
using IdentityService.Application.Interfaces;
using MediatR;

namespace IdentityService.Application.Features.Authentication.ForgotPassword;

public class ForgotPasswordHandler
    : IRequestHandler<ForgotPasswordCommand, bool>
{
    private readonly IUserRepository _userRepository;

    public ForgotPasswordHandler(
        IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<bool> Handle(
        ForgotPasswordCommand request,
        CancellationToken cancellationToken)
    {
        var user =
            await _userRepository
                .GetByEmailAsync(request.Email);

        if (user == null)
        {
            return true;
        }

        var token =
            GenerateResetToken();

        user.PasswordResetToken = token;

        user.PasswordResetTokenExpiry =
            DateTime.UtcNow.AddHours(1);

        await _userRepository.UpdateAsync(user);

        await _userRepository.SaveChangesAsync();

        return true;
    }

    private string GenerateResetToken()
    {
        var bytes = new byte[64];

        using var rng =
            RandomNumberGenerator.Create();

        rng.GetBytes(bytes);

        return Convert.ToBase64String(bytes);
    }
}