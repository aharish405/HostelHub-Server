using HostelHub.Application.Common.Interfaces;
using HostelHub.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HostelHub.Api.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
[Authorize(Policy = "TenantLocked")]
public class BedsController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;

    public BedsController(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        // Use Include to get Room info
        var beds = await _unitOfWork.Beds.Entities
            .Include(b => b.Room)
            .ToListAsync();
            
        // Map to a UI-friendly structure grouped by Room
        var grouped = beds.GroupBy(b => new { b.Room.Id, b.Room.RoomNumber })
            .Select(g => new {
                RoomId = g.Key.Id,
                RoomNumber = g.Key.RoomNumber,
                Beds = g.Select(b => new {
                    b.Id,
                    b.BedName,
                    BedType = b.BedType.ToString(),
                    Status = b.Status.ToString(),
                    b.PricePerNight
                }),
                g.FirstOrDefault()?.Room.AirConditioning
            });

        return Ok(grouped);
    }
}
