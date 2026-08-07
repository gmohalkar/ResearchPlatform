public interface IPermissionRepository
{
    Task<List<string>>
GetPermissionsByRoleIdAsync(
Guid roleId);
}