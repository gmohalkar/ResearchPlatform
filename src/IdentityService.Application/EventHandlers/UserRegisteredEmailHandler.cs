using IdentityService.Application.Events;
using MediatR;

namespace IdentityService.Application.EventHandlers;

public class UserRegisteredEmailHandler
    : INotificationHandler<
        UserRegisteredEvent>
{
    public async Task Handle(
        UserRegisteredEvent notification,
        CancellationToken cancellationToken)
    {
        Console.WriteLine(
            $"Sending welcome email to {notification.Email}");

        await Task.CompletedTask;
    }
}