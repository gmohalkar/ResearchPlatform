using IdentityService.Domain.Entities;

namespace IdentityService.Application.Interfaces;

public interface IRefreshTokenRepository
{
    Task AddAsync(
        RefreshToken refreshToken);

    Task<RefreshToken?> GetByTokenAsync(
        string token);

        Task UpdateAsync(
    RefreshToken refreshToken);


    Task SaveChangesAsync();
}