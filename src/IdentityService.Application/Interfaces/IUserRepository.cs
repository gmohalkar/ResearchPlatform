using IdentityService.Domain.Entities;

public interface IUserRepository
{
    Task<User?> GetByEmailAsync(string email);

    Task AddAsync(User user);

    Task UpdateAsync(User user);

    Task<User?> GetByResetTokenAsync(
    string token);

    Task<User?> GetByVerificationTokenAsync(
    string token);

    Task<User?> GetByIdAsync(Guid userId);

    Task SaveChangesAsync();
}