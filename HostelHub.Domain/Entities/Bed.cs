using System.ComponentModel.DataAnnotations;
using HostelHub.Domain.Common;
using HostelHub.Domain.Enums;

namespace HostelHub.Domain.Entities;

public class Bed : BaseEntity, IMustHaveTenant
{
    public Guid RoomId { get; set; }
    public Room Room { get; set; } = null!;
    
    public string BedNumber { get; set; } = string.Empty;
    public BedStatus Status { get; set; }
    public decimal PricePerNight { get; set; }
    
    // Concurrency tracking for atomic operations
    public byte[] Version { get; set; } = null!;
    
    public string TenantId { get; set; } = string.Empty;
}
