using FluentValidation;
using HostelHub.Application.Common.Interfaces;
using HostelHub.Application.Features.Bookings.Events;
using HostelHub.Domain.Entities;
using HostelHub.Domain.Enums;
using MediatR;

namespace HostelHub.Application.Features.Bookings.Commands.CheckInGuest;

public record CheckInGuestCommand(Guid BookingId) : IRequest<bool>;

public class CheckInGuestCommandHandler : IRequestHandler<CheckInGuestCommand, bool>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMediator _mediator;

    public CheckInGuestCommandHandler(IUnitOfWork unitOfWork, IMediator mediator)
    {
        _unitOfWork = unitOfWork;
        _mediator = mediator;
    }

    public async Task<bool> Handle(CheckInGuestCommand request, CancellationToken cancellationToken)
    {
        // 1. Verify Booking status
        var booking = await _unitOfWork.Bookings.GetByIdAsync(request.BookingId, cancellationToken);
        if (booking == null)
        {
            throw new ArgumentException("Booking not found.");
        }

        if (booking.Status != BookingStatus.Confirmed)
        {
            throw new InvalidOperationException($"Cannot check-in. Booking status is {booking.Status}");
        }

        // 2. Load the bed 
        var bed = await _unitOfWork.Beds.GetByIdAsync(booking.BedId, cancellationToken);
        if (bed == null)
        {
            throw new InvalidOperationException("Assigned bed not found.");
        }

        if (bed.Status != BedStatus.Available)
        {
             // This might happen if someone else took it, EF concurrency token will also protect updates, 
             // but validating early is nice.
             throw new InvalidOperationException("Bed is not available.");
        }

        // 3. Update Statuses
        booking.Status = BookingStatus.CheckedIn;
        bed.Status = BedStatus.Occupied;

        _unitOfWork.Bookings.Update(booking);
        _unitOfWork.Beds.Update(bed);

        // 4. Create a transaction record (Assuming Payment table acts as transaction log for checkin charge or deposit, 
        // alternatively we just generate a transaction ID for tracking)
        var paymentTransaction = new Payment
        {
            BookingId = booking.Id,
            Amount = 0, // e.g. deposit or check-in fee, $0 just for tracing
            Status = PaymentStatus.Completed,
            PaymentDate = DateTime.UtcNow,
            TransactionId = $"CHK-IN-{Guid.NewGuid().ToString("N").Substring(0, 8).ToUpper()}",
            TenantId = booking.TenantId
        };
        await _unitOfWork.Payments.AddAsync(paymentTransaction, cancellationToken);

        // Save atomically
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // 5. Trigger "Welcome" notification event
        await _mediator.Publish(new GuestCheckedInEvent(booking.Id, booking.GuestId, bed.Id), cancellationToken);

        return true;
    }
}

public class CheckInGuestCommandValidator : AbstractValidator<CheckInGuestCommand>
{
    public CheckInGuestCommandValidator()
    {
        RuleFor(v => v.BookingId)
            .NotEmpty();
    }
}
