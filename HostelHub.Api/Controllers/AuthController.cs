using HostelHub.Application.Common.Interfaces;
using HostelHub.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace HostelHub.Api.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class AuthController : ControllerBase
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IJwtService _jwtService;

    public AuthController(UserManager<ApplicationUser> userManager, IJwtService jwtService)
    {
        _userManager = userManager;
        _jwtService = jwtService;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        var user = await _userManager.FindByEmailAsync(request.Email);
        if (user != null && await _userManager.CheckPasswordAsync(user, request.Password))
        {
            var roles = await _userManager.GetRolesAsync(user);
            var token = await _jwtService.GenerateTokenAsync(user.Id, user.Email!, roles, user.TenantId);

            return Ok(new { Token = token });
        }

        return Unauthorized();
    }
}

public record LoginRequest(string Email, string Password);
