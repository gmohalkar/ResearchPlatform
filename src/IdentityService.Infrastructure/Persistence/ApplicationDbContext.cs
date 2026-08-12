using IdentityService.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace IdentityService.Infrastructure.Persistence;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(
        DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<User> Users => Set<User>();

    public DbSet<Role> Roles => Set<Role>();

    public DbSet<RefreshToken> RefreshTokens { get; set; }

    public DbSet<Permission> Permissions => Set<Permission>();

public DbSet<RolePermission> RolePermissions => Set<RolePermission>();

    public DbSet<AuditLog> AuditLogs => Set<AuditLog>();

    public DbSet<OutboxMessage>OutboxMessages => Set<OutboxMessage>();

    protected override void OnModelCreating(
    ModelBuilder modelBuilder)
{
    base.OnModelCreating(modelBuilder);

    modelBuilder.Entity<User>()
        .HasOne(x => x.Role)
        .WithMany(x => x.Users)
        .HasForeignKey(x => x.RoleId);

    modelBuilder.Entity<RefreshToken>()
        .HasOne(x => x.User)
        .WithMany(x => x.RefreshTokens)
        .HasForeignKey(x => x.UserId);
    modelBuilder.Entity<RolePermission>()
    .HasKey(x =>
        new
        {
            x.RoleId,
            x.PermissionId
        });
    modelBuilder.Entity<RolePermission>()
    .HasOne(x => x.Role)
    .WithMany(x => x.RolePermissions)
    .HasForeignKey(x => x.RoleId);
    modelBuilder.Entity<RolePermission>()
    .HasOne(x => x.Permission)
    .WithMany(x => x.RolePermissions)
    .HasForeignKey(x => x.PermissionId);
}
}