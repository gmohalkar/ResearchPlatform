namespace IdentityService.Application.Common;

public interface IDomainEvent
{
    DateTime OccurredOn
    {
        get;
    }
}