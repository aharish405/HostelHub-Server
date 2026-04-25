using HostelHub.Application.Common.Interfaces;

namespace HostelHub.Infrastructure.Services;

public class TenantService : ITenantService
{
    private string? _tenantId;

    public string? GetTenantId()
    {
        return _tenantId;
    }

    public void SetTenantId(string tenantId)
    {
        _tenantId = tenantId;
    }
}
