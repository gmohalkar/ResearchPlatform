using IdentityService.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

public class PermissionRepository : IPermissionRepository
{
    private readonly ApplicationDbContext _context;

    public PermissionRepository(
        ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<string>> GetPermissionsByRoleIdAsync(
        Guid roleId)
    {
       return await _context.RolePermissions
    .Where(x => x.RoleId == roleId)
    .Select(x => x.Permission.Name).ToListAsync();
    }
}