using HostelHub.Application.Common.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HostelHub.Api.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
[Authorize(Policy = "TenantLocked")]
public class GuestsController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;

    public GuestsController(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var guests = await _unitOfWork.Guests.Entities
            .Include(g => g.Bookings)
            .ToListAsync();
            
        return Ok(guests);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var guest = await _unitOfWork.Guests.Entities
            .Include(g => g.Bookings)
            .FirstOrDefaultAsync(g => g.Id == id);
            
        if (guest == null) return NotFound();
        return Ok(guest);
    }
}
