using IdentityService.Domain.Entities;

public interface IAuditLogRepository
{
    Task AddAsync(
        AuditLog auditLog);

    Task SaveChangesAsync();
}