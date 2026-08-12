using IdentityService.Application.Interfaces;
using IdentityService.Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;

namespace IdentityService.Application.Features.Authentication.LoginUser;

public class LoginUserHandler
    : IRequestHandler<LoginUserCommand, LoginResponse>
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordService _passwordService;
    private readonly IJwtService _jwtService;
    private readonly IRefreshTokenService _refreshTokenService;
    private readonly IRefreshTokenRepository _refreshTokenRepository;
    private readonly IPermissionRepository _permissionRepository;
    private readonly IAuditService _auditService;

    private readonly ILogger<LoginUserHandler> _logger;


    public LoginUserHandler(
        IUserRepository userRepository,
        IPasswordService passwordService,
        IJwtService jwtService,
        IRefreshTokenService refreshTokenService,
        IRefreshTokenRepository refreshTokenRepository,
        IPermissionRepository permissionRepository,
        IAuditService auditService,
        ILogger<LoginUserHandler> logger)
    {
        _userRepository = userRepository;
        _passwordService = passwordService;
        _jwtService = jwtService;
        _refreshTokenService = refreshTokenService;
        _refreshTokenRepository = refreshTokenRepository;
        _permissionRepository = permissionRepository;
        _auditService = auditService;
        _logger = logger;
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
        if (
            user.IsLocked && user.LockoutEndTime.HasValue &&
            user.LockoutEndTime > DateTime.UtcNow)
        {
            throw new Exception(
                $"Account is locked until {user.LockoutEndTime}");
        }
        if (
 !_passwordService.VerifyPassword(
    request.Password,
    user.PasswordHash))
        {
            user.FailedLoginAttempts++;

            if (user.FailedLoginAttempts >= 5)
            {
                user.IsLocked = true;
                user.LockoutEndTime =
                    DateTime.UtcNow.AddMinutes(30);
            }

            await _userRepository.UpdateAsync(user);

            await _userRepository.SaveChangesAsync();

            throw new Exception(
                "Invalid email or password.");
        }
        user.FailedLoginAttempts = 0;

        user.LockoutEndTime = null;

        user.LastLoginDate =
            DateTime.UtcNow;
        user.IsLocked = false;

        await _userRepository.UpdateAsync(user);
        await _refreshTokenRepository
           .SaveChangesAsync();

        if (!user.IsEmailVerified)
        {
            throw new Exception(
            "Email address is not verified.");
        }

        var permissions =
    await _permissionRepository
        .GetPermissionsByRoleIdAsync(
            user.RoleId);

        var accessToken =
    _jwtService.GenerateToken(user, permissions);

        var refreshToken =
            _refreshTokenService
                .GenerateRefreshToken();

        await _refreshTokenRepository.AddAsync(
            new Domain.Entities.RefreshToken
            {
                Token = refreshToken,
                UserId = user.Id,
                Expires =
                    DateTime.UtcNow.AddDays(7),
                IsRevoked = false
            });

        await _refreshTokenRepository
            .SaveChangesAsync();

        await _auditService.LogAsync(
            user.Id,
            "Login",
            "/api/auth/login",
            "User logged in successfully");
        _logger.LogInformation(
    "User {Email} logged in successfully",
    request.Email);
        return new LoginResponse
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken,
            ExpiresAt =
         DateTime.UtcNow.AddMinutes(60),
            Role = user.Role?.Name ?? string.Empty,
            Email = user.Email
        };
    }
}