using IdentityService.Domain.Entities;

public interface IJwtService
{
    string GenerateToken(User user);
}