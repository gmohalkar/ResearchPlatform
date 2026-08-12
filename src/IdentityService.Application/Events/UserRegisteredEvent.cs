using MediatR;

namespace IdentityService.Application.Events;

public class UserRegisteredEvent: INotification
{
    public Guid UserId { get; set; }

    public string Email { get; set; }
        = string.Empty;

    public string FirstName { get; set; }
        = string.Empty;

    public DateTime OccurredOn
    {
        get;
        set;
    }
}