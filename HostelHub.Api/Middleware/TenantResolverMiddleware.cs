using HostelHub.Application.Common.Interfaces;

namespace HostelHub.Api.Middleware;

public class TenantResolverMiddleware
{
    private readonly RequestDelegate _next;

    public TenantResolverMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context, ITenantService tenantService)
    {
        context.Request.Headers.TryGetValue("X-Tenant-ID", out var tenantFromHeader);
        
        if (!string.IsNullOrEmpty(tenantFromHeader))
        {
            tenantService.SetTenantId(tenantFromHeader!);
        }

        await _next(context);
    }
}
