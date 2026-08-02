using IdentityService.Domain.Entities;

namespace IdentityService.Application.Interfaces;

public interface IRoleRepository
{
    Task<Role?> GetByNameAsync(string roleName);

    Task<Role?> GetByIdAsync(Guid roleId);

    Task<List<Role>> GetAllAsync();
}