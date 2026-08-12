namespace IdentityService.Domain.Entities;

public class OutboxMessage : BaseEntity
{
    public string Type
    {
        get;
        set;
    } = string.Empty;

    public string Content
    {
        get;
        set;
    } = string.Empty;

    public DateTime CreatedOn
    {
        get;
        set;
    }

    public DateTime? ProcessedOn
    {
        get;
        set;
    }
}