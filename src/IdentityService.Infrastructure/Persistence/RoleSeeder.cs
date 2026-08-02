using IdentityService.Domain.Entities;

namespace IdentityService.Infrastructure.Persistence;

public static class RoleSeeder
{
    public static async Task SeedAsync(
        ApplicationDbContext context)
    {
        Console.WriteLine("Role Seeder Started");

        if (context.Roles.Any())
        {
            Console.WriteLine("Roles already exist");
            return;
        }

        Console.WriteLine("Creating roles...");
        if (context.Roles.Any())
            return;

        var roles = new List<Role>
        {
            new Role
            {
                Id = Guid.NewGuid(),
                Name = "Admin"
            },
            new Role
            {
                Id = Guid.NewGuid(),
                Name = "Researcher"
            },
            new Role
            {
                Id = Guid.NewGuid(),
                Name = "Reviewer"
            },
            new Role
            {
                Id = Guid.NewGuid(),
                Name = "Guest"
            }
        };

        await context.Roles.AddRangeAsync(roles);

        await context.SaveChangesAsync();
    }
}