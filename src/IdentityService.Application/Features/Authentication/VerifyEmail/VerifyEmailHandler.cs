using IdentityService.Application.Interfaces;
using MediatR;

namespace IdentityService.Application.Features.Authentication.VerifyEmail;

public class VerifyEmailHandler
    : IRequestHandler<VerifyEmailCommand, bool>
{
    private readonly IUserRepository _userRepository;

    public VerifyEmailHandler(
        IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<bool> Handle(
        VerifyEmailCommand request,
        CancellationToken cancellationToken)
    {
        var user =
            await _userRepository
                .GetByVerificationTokenAsync(
                    request.Token);

        if (user == null)
        {
            throw new Exception(
                "Invalid verification token.");
        }

        user.IsEmailVerified = true;

        user.EmailVerificationToken = null;

        await _userRepository.UpdateAsync(user);

        await _userRepository.SaveChangesAsync();

        return true;
    }
}