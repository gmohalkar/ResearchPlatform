using IdentityService.Domain.Entities;
using IdentityService.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

public class UserRepository : IUserRepository
{
    private readonly ApplicationDbContext _context;

    public UserRepository(
        ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(User user)
    {
        await _context.Users.AddAsync(user);
    }

    public Task UpdateAsync(User user)
{
_context.Users.Update(user);
return Task.CompletedTask;
}

  public async Task<User?> GetByEmailAsync(
    string email)
{
    return await _context.Users
        .Include(x => x.Role)
        .FirstOrDefaultAsync(x =>
            x.Email == email);
}

public async Task<User?> GetByResetTokenAsync(
    string token)
{
    return await _context.Users
        .FirstOrDefaultAsync(x =>
            x.PasswordResetToken == token);
}

public async Task<User?>
GetByVerificationTokenAsync(
    string token)
{
    return await _context.Users
        .FirstOrDefaultAsync(x =>
            x.EmailVerificationToken == token);
}

public async Task<User?> GetByIdAsync(
    Guid userId)
{
    return await _context.Users
        .Include(x => x.Role)
        .FirstOrDefaultAsync(x =>
            x.Id == userId);
}
    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    } 
}