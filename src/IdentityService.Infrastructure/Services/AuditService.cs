using IdentityService.Application.Interfaces;
using IdentityService.Domain.Entities;

public class AuditService
    : IAuditService
{
    private readonly IAuditLogRepository
        _auditLogRepository;

    public AuditService(
        IAuditLogRepository
            auditLogRepository)
    {
        _auditLogRepository =
            auditLogRepository;
    }

    public async Task LogAsync(
        Guid? userId,
        string action,
        string endpoint,
        string details)
    {
        await _auditLogRepository
            .AddAsync(
                new AuditLog
                {
                    UserId = userId,
                    Action = action,
                    Endpoint = endpoint,
                    Details = details,
                    Timestamp =
                        DateTime.UtcNow
                });

        await _auditLogRepository
            .SaveChangesAsync();
    }
}