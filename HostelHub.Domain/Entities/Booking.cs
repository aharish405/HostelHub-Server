using HostelHub.Domain.Common;
using HostelHub.Domain.Enums;

namespace HostelHub.Domain.Entities;

public class Booking : BaseEntity, IMustHaveTenant
{
    public Guid BedId { get; set; }
    public Bed Bed { get; set; } = null!;
    
    public Guid GuestId { get; set; } // Reference to User/Guest table later
    
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public BookingStatus Status { get; set; }
    
    public ICollection<Payment> Payments { get; set; } = new List<Payment>();

    public string TenantId { get; set; } = string.Empty;
}
