using IdentityService.Domain.Entities;

public interface IOutboxRepository
{
    Task AddAsync(
        OutboxMessage message);

    Task SaveChangesAsync();

    Task<List<OutboxMessage>>
        GetUnprocessedMessagesAsync();
}