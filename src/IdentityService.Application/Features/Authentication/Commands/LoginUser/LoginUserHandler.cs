using IdentityService.Application.Interfaces;
using MediatR;

namespace IdentityService.Application.Features.Authentication.LoginUser;

public class LoginUserHandler
    : IRequestHandler<LoginUserCommand, LoginResponse>
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordService _passwordService;
    private readonly IJwtService _jwtService;

    public LoginUserHandler(
        IUserRepository userRepository,
        IPasswordService passwordService,
        IJwtService jwtService)
    {
        _userRepository = userRepository;
        _passwordService = passwordService;
        _jwtService = jwtService;
    }

    public async Task<LoginResponse> Handle(
        LoginUserCommand request,
        CancellationToken cancellationToken)
    {
        var user = await _userRepository
            .GetByEmailAsync(request.Email);

        if (user == null)
        {
            throw new Exception("Invalid email or password.");
        }

        var isValidPassword =
            _passwordService.VerifyPassword(
                request.Password,
                user.PasswordHash);

        if (!isValidPassword)
        {
            throw new Exception("Invalid email or password.");
        }

        var token =
            _jwtService.GenerateToken(user);

        return new LoginResponse
        {
            Token = token,
            Email = user.Email,
            Role = user.Role?.Name ?? string.Empty,
            ExpiresAt = DateTime.UtcNow.AddMinutes(60)
        };
    }
}