using IdentityService.Application.Interfaces;
using IdentityService.Domain.Entities;
using MediatR;

public class RegisterUserHandler
: IRequestHandler<RegisterUserCommand, Guid>
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordService _passwordService;
    private readonly IRoleRepository _roleRepository;
    private readonly IResetPasswordTokenService _resetPasswordTokenService;
    public RegisterUserHandler(
     IUserRepository userRepository,
     IPasswordService passwordService,
     IRoleRepository roleRepository, IResetPasswordTokenService resetPasswordTokenService)
    {
        _userRepository = userRepository;
        _passwordService = passwordService;
        _roleRepository = roleRepository;
        _resetPasswordTokenService = resetPasswordTokenService;
    }
    public async Task<Guid> Handle(
    RegisterUserCommand request,
    CancellationToken cancellationToken)
    {
        var existingUser =
        await _userRepository.GetByEmailAsync(
        request.Email);

        var role =
       await _roleRepository
           .GetByNameAsync(
               request.RoleName);

        if (role == null)
        {
            throw new Exception(
                $"Role '{request.RoleName}' does not exist.");
        }
        if (existingUser != null)
        {
            throw new Exception(
            $"User already exists with email {request.Email}");
        }

        var verificationToken =
    _resetPasswordTokenService
        .GenerateToken();

        var user = new User
        {
            Id = Guid.NewGuid(),
            FirstName = request.FirstName,
            LastName = request.LastName,
            Email = request.Email,
            PasswordHash =
        _passwordService.HashPassword(
            request.Password),
            RoleId = role.Id,
            IsEmailVerified = false,
            EmailVerificationToken =
verificationToken
        };
        await _userRepository.AddAsync(user);
        await _userRepository.SaveChangesAsync();
        return user.Id;
    }
}