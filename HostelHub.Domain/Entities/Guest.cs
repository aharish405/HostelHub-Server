using HostelHub.Domain.Common;

namespace HostelHub.Domain.Entities;

public class Guest : BaseEntity, IMustHaveTenant
{
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public string? IdentificationNumber { get; set; }
    
    public ICollection<Booking> Bookings { get; set; } = new List<Booking>();
    
    public string TenantId { get; set; } = string.Empty;
}
