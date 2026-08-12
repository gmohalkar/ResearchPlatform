using IdentityService.Application.Events;
using MediatR;

namespace IdentityService.Application.EventHandlers;

public class UserRegisteredNotificationHandler
    : INotificationHandler<
        UserRegisteredEvent>
{
    public async Task Handle(
        UserRegisteredEvent notification,
        CancellationToken cancellationToken)
    {
        Console.WriteLine(
            $"Notification Sent To {notification.Email}");

        await Task.CompletedTask;
    }
}