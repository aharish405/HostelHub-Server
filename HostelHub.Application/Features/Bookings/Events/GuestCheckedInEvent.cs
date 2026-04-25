using MediatR;

namespace HostelHub.Application.Features.Bookings.Events;

public record GuestCheckedInEvent(Guid BookingId, Guid GuestId, Guid BedId) : INotification;

public class GuestCheckedInEventHandler : INotificationHandler<GuestCheckedInEvent>
{
    // Normally you'd inject an IEmailService or INotificationService here
    public Task Handle(GuestCheckedInEvent notification, CancellationToken cancellationToken)
    {
        // Simulate sending a "Welcome" notification event
        Console.WriteLine($"[EVENT] Welcome notification sent to Guest {notification.GuestId} for checking into Bed {notification.BedId}.");
        return Task.CompletedTask;
    }
}
