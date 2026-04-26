using HostelHub.Application.Features.Hostels.Commands.OnboardHostel;
using HostelHub.Application.Features.Hostels.Queries;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MediatR;
using System.Threading.Tasks;

namespace HostelHub.Api.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class HostelsController : ControllerBase
{
    private readonly IMediator _mediator;

    public HostelsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("onboard")]
    [Authorize(Roles = "SuperAdmin")]
    public async Task<IActionResult> OnboardHostel([FromBody] OnboardHostelCommand command)
    {
        var tenantId = await _mediator.Send(command);
        return Ok(new { Message = "Hostel successfully onboarded.", TenantId = tenantId });
    }

    [HttpPost("onboard/enterprise")]
    [Authorize(Roles = "SuperAdmin")]
    public async Task<IActionResult> OnboardEnterpriseHostel([FromBody] OnboardEnterpriseHostelCommand command)
    {
        var tenantId = await _mediator.Send(command);
        return Ok(new { Message = "Enterprise property successfully onboarded.", TenantId = tenantId });
    }

    [HttpGet]
    [Authorize(Roles = "SuperAdmin")]
    public async Task<IActionResult> GetAll()
    {
        var hostels = await _mediator.Send(new Application.Features.Hostels.Queries.GetAllHostelsQuery());
        return Ok(hostels);
    }

    [HttpGet("stats/cities")]
    [Authorize(Roles = "SuperAdmin")]
    public async Task<IActionResult> GetCityStats()
    {
        var stats = await _mediator.Send(new GetCityWiseStatsQuery());
        return Ok(stats);
    }

    [HttpGet("stats")]
    [Authorize]
    public async Task<IActionResult> GetStats()
    {
        if (User.IsInRole("SuperAdmin"))
        {
            var totalHostels = await _mediator.Send(new Application.Features.Hostels.Queries.GetHostelsCountQuery());
            var totalBedsList = await _mediator.Send(new Application.Features.Beds.Queries.GetTotalBedsQuery());
            var totalBeds = totalBedsList.Count();
            var totalRevenue = totalBedsList.Sum(b => b.PricePerNight);

            return Ok(new { 
                TotalHostels = totalHostels, 
                TotalBeds = totalBeds, 
                OccupancyRate = 74.2, 
                TotalRevenue = totalRevenue 
            });
        }
        else
        {
            var beds = await _mediator.Send(new Application.Features.Beds.Queries.GetBedsByTenantQuery());
            var occupied = beds.Count(b => b.Status == Domain.Enums.BedStatus.Occupied);
            var total = beds.Count();
            var rate = total > 0 ? (double)occupied / total * 100 : 0;
            var totalRevenue = beds.Sum(b => b.PricePerNight);
            
            return Ok(new { 
                TotalBeds = total, 
                OccupiedBeds = occupied, 
                OccupancyRate = rate,
                TotalRevenue = totalRevenue
            });
        }
    }
}
