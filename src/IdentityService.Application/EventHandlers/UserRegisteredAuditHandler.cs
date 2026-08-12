using IdentityService.Application.Events;
using MediatR;

namespace IdentityService.Application.EventHandlers;

public class UserRegisteredAuditHandler
    : INotificationHandler<
        UserRegisteredEvent>
{
    public async Task Handle(
        UserRegisteredEvent notification,
        CancellationToken cancellationToken)
    {
        Console.WriteLine(
            $"Audit Created For {notification.Email}");

        await Task.CompletedTask;
    }
}