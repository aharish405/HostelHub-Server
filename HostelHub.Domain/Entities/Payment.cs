using HostelHub.Domain.Common;
using HostelHub.Domain.Enums;

namespace HostelHub.Domain.Entities;

public class Payment : BaseEntity, IMustHaveTenant
{
    public Guid BookingId { get; set; }
    public Booking Booking { get; set; } = null!;
    
    public decimal Amount { get; set; }
    public DateTime PaymentDate { get; set; }
    public PaymentStatus Status { get; set; }
    public string TransactionId { get; set; } = string.Empty;

    public string TenantId { get; set; } = string.Empty;
}
