using IdentityService.Application.Interfaces;
using IdentityService.Domain.Entities;
using MediatR;

public class RegisterUserHandler
: IRequestHandler<RegisterUserCommand, Guid>
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordService _passwordService;
    private readonly IRoleRepository _roleRepository;
    public RegisterUserHandler(
     IUserRepository userRepository,
     IPasswordService passwordService,
     IRoleRepository roleRepository)
    {
        _userRepository = userRepository;
        _passwordService = passwordService;
        _roleRepository = roleRepository;
    }
    public async Task<Guid> Handle(
    RegisterUserCommand request,
    CancellationToken cancellationToken)
    {
        var existingUser =
        await _userRepository.GetByEmailAsync(
        request.Email);
        
        var defaultRole =
    await _roleRepository
        .GetByNameAsync("Researcher");
Console.WriteLine($"Role Found: {defaultRole?.Name}");
Console.WriteLine($"Role Id: {defaultRole?.Id}");
        if (defaultRole == null)
        {
            throw new Exception(
                "Researcher role not found.");
        }
        if (existingUser != null)
        {
            throw new Exception(
            $"User already exists with email {request.Email}");
        }
        var user = new User
{
    Id = Guid.NewGuid(),
    FirstName = request.FirstName,
    LastName = request.LastName,
    Email = request.Email,
    PasswordHash =
        _passwordService.HashPassword(
            request.Password),

    RoleId = defaultRole.Id
};
        await _userRepository.AddAsync(user);
        await _userRepository.SaveChangesAsync();
        return user.Id;
    }
}