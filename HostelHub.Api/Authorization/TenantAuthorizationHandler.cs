using System.Text;
using HostelHub.Domain.Constants;
using HostelHub.Infrastructure.Identity;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;

namespace HostelHub.Api.Authorization;

// We enforce explicit matching of Hostels Tenant vs the User's Tenant Claim
public class TenantRequirement : IAuthorizationRequirement
{
    public bool MustMatch { get; }
    
    public TenantRequirement(bool mustMatch = true)
    {
        MustMatch = mustMatch;
    }
}

public class TenantAuthorizationHandler : AuthorizationHandler<TenantRequirement>
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public TenantAuthorizationHandler(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, TenantRequirement requirement)
    {
        // SuperAdmins bypass tenant checks naturally as they span the system natively
        if (context.User.IsInRole(Roles.SuperAdmin))
        {
            context.Succeed(requirement);
            return Task.CompletedTask;
        }

        if (requirement.MustMatch)
        {
            var userTenantClaim = context.User.FindFirst("TenantId")?.Value;
            var requestTenantHeader = _httpContextAccessor.HttpContext?.Request.Headers["X-Tenant-ID"].ToString();

            if (!string.IsNullOrEmpty(userTenantClaim) && userTenantClaim == requestTenantHeader)
            {
                context.Succeed(requirement);
            }
        }
        else
        {
            context.Succeed(requirement);
        }

        return Task.CompletedTask;
    }
}
