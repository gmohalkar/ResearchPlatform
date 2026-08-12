using IdentityService.Domain.Entities;
using IdentityService.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

public class OutboxRepository
    : IOutboxRepository
{
    private readonly ApplicationDbContext _context;

    public OutboxRepository(
        ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(
        OutboxMessage message)
    {
        await _context.OutboxMessages
            .AddAsync(message);
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }

    public async Task<List<OutboxMessage>>
        GetUnprocessedMessagesAsync()
    {
        return await _context.OutboxMessages
            .Where(x =>
                x.ProcessedOn == null).ToListAsync();
    }
}