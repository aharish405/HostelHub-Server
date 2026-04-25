using HostelHub.Application.Features.Hostels.Commands.OnboardHostel;
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
    [Authorize(Roles = "SuperAdmin")] // Secure this endpoint specifically for SuperAdmins
    public async Task<IActionResult> OnboardHostel([FromBody] OnboardHostelCommand command)
    {
        var tenantId = await _mediator.Send(command);
        return Ok(new { Message = "Hostel successfully onboarded.", TenantId = tenantId });
    }
}
