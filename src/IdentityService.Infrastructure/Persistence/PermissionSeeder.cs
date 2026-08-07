using IdentityService.Domain.Entities;
using IdentityService.Infrastructure.Persistence;

public static class PermissionSeeder
{
    public static async Task SeedAsync(
        ApplicationDbContext context)
    {
        if(context.Permissions.Any())
            return;

        context.Permissions.AddRange(
            new Permission
            {
                Name="CreateUser"
            },
            new Permission
            {
                Name="DeleteUser"
            },
            new Permission
            {
                Name="ManageRoles"
            },
            new Permission
            {
                Name="ViewAuditLogs"
            });

        await context.SaveChangesAsync();
    }
}