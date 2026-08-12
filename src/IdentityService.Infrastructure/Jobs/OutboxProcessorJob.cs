public class OutboxProcessorJob
{
    private readonly
        IOutboxRepository
            _outboxRepository;

    public OutboxProcessorJob(
        IOutboxRepository
            outboxRepository)
    {
        _outboxRepository =
            outboxRepository;
    }

    public async Task Execute()
    {
        var messages =
            await _outboxRepository
                .GetUnprocessedMessagesAsync();

        foreach(var message
            in messages)
        {
            Console.WriteLine(
                $"Publishing {message.Type}");

            message.ProcessedOn =
                DateTime.UtcNow;
        }

        await _outboxRepository
            .SaveChangesAsync();
    }
}