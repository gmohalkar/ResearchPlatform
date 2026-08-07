using IdentityService.Infrastructure.Persistence;

public class CleanupJob
{
    private readonly ApplicationDbContext
        _context;

    public CleanupJob(
        ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task Execute()
    {
        var expiredTokens =
            _context.RefreshTokens
                .Where(x =>
                    x.Expires <
                        DateTime.UtcNow);

        _context.RefreshTokens
            .RemoveRange(expiredTokens);

        var users =
    _context.Users
        .Where(x =>
            x.PasswordResetTokenExpiry
                < DateTime.UtcNow);

        _context.Users
            .RemoveRange(users);

        await _context.SaveChangesAsync();
    }
}