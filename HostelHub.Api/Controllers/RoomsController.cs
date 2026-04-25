using HostelHub.Application.Common.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HostelHub.Api.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
[Authorize(Policy = "TenantLocked")]
public class RoomsController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;

    public RoomsController(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    [HttpGet]
    public async Task<IActionResult> GetRooms()
    {
        // The Global Query Filter in ApplicationDbContext will handle the filtering
        // only allowing rooms from the TenantId present in the X-Tenant-ID header
        // which must match the User's TenantId claim (checked by the policy).
        var rooms = await _unitOfWork.Rooms.GetAllAsync();
        return Ok(rooms);
    }
}
