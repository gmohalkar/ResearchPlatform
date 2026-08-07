public interface IAuditService
{
    Task LogAsync(
        Guid? userId,
        string action,
        string endpoint,
        string details);
}