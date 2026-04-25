using HostelHub.Application.Features.Bookings.Commands.CreateBooking;
using HostelHub.Application.Features.Bookings.Queries.GetAvailability;
using HostelHub.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HostelHub.Api.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
[Authorize(Policy = "TenantLocked")]
public class BookingsController : ControllerBase
{
    private readonly IMediator _mediator;

    public BookingsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("availability")]
    public async Task<IActionResult> GetAvailability(
        [FromQuery] Guid hostelId, 
        [FromQuery] DateTime startDate, 
        [FromQuery] DateTime endDate, 
        [FromQuery] RoomType? roomType)
    {
        var query = new GetAvailabilityQuery(hostelId, startDate, endDate, roomType);
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> CreateBooking([FromBody] CreateBookingCommand command)
    {
        var bookingId = await _mediator.Send(command);
        return Ok(new { BookingId = bookingId, Message = "Booking created successfully" });
    }
}
