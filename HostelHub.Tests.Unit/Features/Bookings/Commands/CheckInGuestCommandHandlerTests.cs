using FluentAssertions;
using HostelHub.Application.Common.Interfaces;
using HostelHub.Application.Features.Bookings.Commands.CheckInGuest;
using HostelHub.Application.Features.Bookings.Events;
using HostelHub.Domain.Entities;
using HostelHub.Domain.Enums;
using MediatR;
using NSubstitute;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace HostelHub.Tests.Unit.Features.Bookings.Commands;

public class CheckInGuestCommandHandlerTests
{
    private readonly IUnitOfWork _unitOfWorkMock;
    private readonly IMediator _mediatorMock;
    private readonly CheckInGuestCommandHandler _handler;

    public CheckInGuestCommandHandlerTests()
    {
        _unitOfWorkMock = Substitute.For<IUnitOfWork>();
        _mediatorMock = Substitute.For<IMediator>();

        _handler = new CheckInGuestCommandHandler(_unitOfWorkMock, _mediatorMock);
    }

    [Fact]
    public async Task Handle_BookingNotFound_ThrowsArgumentException()
    {
        // Arrange
        var command = new CheckInGuestCommand(Guid.NewGuid());
        _unitOfWorkMock.Bookings.GetByIdAsync(command.BookingId, Arg.Any<CancellationToken>())
            .Returns((Booking?)null);

        // Act
        Func<Task> action = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        await action.Should().ThrowAsync<ArgumentException>().WithMessage("Booking not found.");
    }

    [Fact]
    public async Task Handle_BookingNotConfirmed_ThrowsInvalidOperationException()
    {
        // Arrange
        var command = new CheckInGuestCommand(Guid.NewGuid());
        var booking = new Booking { Id = command.BookingId, Status = BookingStatus.Pending };
        
        _unitOfWorkMock.Bookings.GetByIdAsync(command.BookingId, Arg.Any<CancellationToken>())
            .Returns(booking);

        // Act
        Func<Task> action = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        await action.Should().ThrowAsync<InvalidOperationException>().WithMessage("Cannot check-in. Booking status is Pending");
    }

    [Fact]
    public async Task Handle_BedNotAvailable_ThrowsInvalidOperationException()
    {
        // Arrange
        var command = new CheckInGuestCommand(Guid.NewGuid());
        var booking = new Booking { Id = command.BookingId, BedId = Guid.NewGuid(), Status = BookingStatus.Confirmed };
        var bed = new Bed { Id = booking.BedId, Status = BedStatus.Occupied };
        
        _unitOfWorkMock.Bookings.GetByIdAsync(command.BookingId, Arg.Any<CancellationToken>()).Returns(booking);
        _unitOfWorkMock.Beds.GetByIdAsync(booking.BedId, Arg.Any<CancellationToken>()).Returns(bed);

        // Act
        Func<Task> action = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        await action.Should().ThrowAsync<InvalidOperationException>().WithMessage("Bed is not available.");
    }

    [Fact]
    public async Task Handle_ValidRequest_ChecksInSuccessfully()
    {
        // Arrange
        var tenantId = "tenant-123";
        var command = new CheckInGuestCommand(Guid.NewGuid());
        var booking = new Booking 
        { 
            Id = command.BookingId, 
            BedId = Guid.NewGuid(), 
            GuestId = Guid.NewGuid(),
            Status = BookingStatus.Confirmed,
            TenantId = tenantId
        };
        var bed = new Bed { Id = booking.BedId, Status = BedStatus.Available };
        
        _unitOfWorkMock.Bookings.GetByIdAsync(command.BookingId, Arg.Any<CancellationToken>()).Returns(booking);
        _unitOfWorkMock.Beds.GetByIdAsync(booking.BedId, Arg.Any<CancellationToken>()).Returns(bed);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().BeTrue();
        
        booking.Status.Should().Be(BookingStatus.CheckedIn);
        bed.Status.Should().Be(BedStatus.Occupied);

        _unitOfWorkMock.Bookings.Received(1).Update(booking);
        _unitOfWorkMock.Beds.Received(1).Update(bed);
        await _unitOfWorkMock.Payments.Received(1).AddAsync(Arg.Is<Payment>(p => 
            p.BookingId == booking.Id && 
            p.Status == PaymentStatus.Completed && 
            p.TenantId == tenantId), Arg.Any<CancellationToken>());
            
        await _unitOfWorkMock.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());

        await _mediatorMock.Received(1).Publish(
            Arg.Is<GuestCheckedInEvent>(e => e.BookingId == booking.Id && e.GuestId == booking.GuestId && e.BedId == bed.Id), 
            Arg.Any<CancellationToken>());
    }
}
