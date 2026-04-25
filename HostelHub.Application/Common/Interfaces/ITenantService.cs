namespace HostelHub.Application.Common.Interfaces;

public interface ITenantService
{
    string? GetTenantId();
    void SetTenantId(string tenantId);
}
