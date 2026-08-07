using IdentityService.Application.Interfaces;
using IdentityService.Domain.Entities;
using IdentityService.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace IdentityService.Infrastructure.Repositories;

public class RefreshTokenRepository
    : IRefreshTokenRepository
{
    private readonly ApplicationDbContext _context;

    public RefreshTokenRepository(
        ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(
        RefreshToken refreshToken)
    {
        await _context.RefreshTokens
            .AddAsync(refreshToken);
    }

    public async Task<RefreshToken?> GetByTokenAsync(
        string token)
    {
        return await _context.RefreshTokens
            .Include(x => x.User)
            .FirstOrDefaultAsync(x =>
                x.Token == token);
    }

public Task UpdateAsync(
    RefreshToken refreshToken)
{
    _context.RefreshTokens
        .Update(refreshToken);
return Task.CompletedTask;
}

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}