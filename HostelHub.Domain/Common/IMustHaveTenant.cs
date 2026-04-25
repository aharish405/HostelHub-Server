namespace HostelHub.Domain.Common;

public interface IMustHaveTenant
{
    string TenantId { get; set; }
}
