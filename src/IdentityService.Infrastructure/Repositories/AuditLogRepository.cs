using IdentityService.Application.Interfaces;
using IdentityService.Domain.Entities;
using IdentityService.Infrastructure.Persistence;

public class AuditLogRepository
    : IAuditLogRepository
{
    private readonly ApplicationDbContext
        _context;

    public AuditLogRepository(
        ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(
        AuditLog auditLog)
    {
        await _context.AuditLogs
            .AddAsync(auditLog);
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}