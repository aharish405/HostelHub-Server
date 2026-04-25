using FluentValidation;
using HostelHub.Application.Common.Interfaces;
using HostelHub.Domain.Entities;
using HostelHub.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HostelHub.Application.Features.Bookings.Commands.CreateBooking;

public record CreateBookingCommand(
    Guid BedId,
    DateTime StartDate,
    DateTime EndDate,
    Guid GuestId) : IRequest<Guid>;

public class CreateBookingCommandHandler : IRequestHandler<CreateBookingCommand, Guid>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IAvailabilityService _availabilityService;
    private readonly ITenantService _tenantService;

    public CreateBookingCommandHandler(
        IUnitOfWork unitOfWork, 
        IAvailabilityService availabilityService, 
        ITenantService tenantService)
    {
        _unitOfWork = unitOfWork;
        _availabilityService = availabilityService;
        _tenantService = tenantService;
    }

    public async Task<Guid> Handle(CreateBookingCommand request, CancellationToken cancellationToken)
    {
        var tenantId = _tenantService.GetTenantId();
        if (string.IsNullOrEmpty(tenantId)) throw new UnauthorizedAccessException();

        // 1. Get the bed to check its room and hostel
        var bed = await _unitOfWork.Beds.GetByIdAsync(request.BedId, cancellationToken);
        if (bed == null) throw new ArgumentException("Bed not found");

        var room = await _unitOfWork.Rooms.GetByIdAsync(bed.RoomId, cancellationToken);
        if (room == null) throw new InvalidOperationException("Room context lost");

        // 2. Perform Final Availability Check (Double check to avoid race conditions before attempting update)
        var availables = await _availabilityService.GetAvailableBedsAsync(
            room.HostelId, request.StartDate, request.EndDate, null, tenantId);
        
        if (!availables.Any(a => a.BedId == request.BedId))
        {
            throw new InvalidOperationException("Bed is no longer available for these dates.");
        }

        // 3. Create Booking
        var booking = new Booking
        {
            BedId = request.BedId,
            GuestId = request.GuestId,
            StartDate = request.StartDate,
            EndDate = request.EndDate,
            Status = BookingStatus.Confirmed,
            TenantId = tenantId
        };

        // We mark the bed as updated to trigger the RowVersion check if someone else is editing the bed record
        // though usually, the existence of the Booking record is the "locking" mechanism in our availability query.
        // To be safe, we can update the bed status or just a dummy property if necessary.
        // But the primary protection is the availability query check which checks the Bookings table.
        
        await _unitOfWork.Bookings.AddAsync(booking, cancellationToken);

        try
        {
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateConcurrencyException)
        {
            throw new InvalidOperationException("A concurrency conflict occurred. Please try again.");
        }

        return booking.Id;
    }
}

public class CreateBookingCommandValidator : AbstractValidator<CreateBookingCommand>
{
    public CreateBookingCommandValidator()
    {
        RuleFor(x => x.BedId).NotEmpty();
        RuleFor(x => x.GuestId).NotEmpty();
        RuleFor(x => x.StartDate).NotEmpty().LessThan(x => x.EndDate);
        RuleFor(x => x.EndDate).NotEmpty().GreaterThan(x => x.StartDate);
        RuleFor(x => x.StartDate).GreaterThanOrEqualTo(DateTime.Today).WithMessage("Start date cannot be in the past");
    }
}
