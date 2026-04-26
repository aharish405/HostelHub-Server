using HostelHub.Domain.Common;
using HostelHub.Domain.Enums;

namespace HostelHub.Domain.Entities;

public class Room : BaseEntity, IMustHaveTenant
{
    public Guid HostelId { get; set; }
    public Hostel Hostel { get; set; } = null!;
    
    public string RoomNumber { get; set; } = string.Empty;
    public int FloorNumber { get; set; }
    public RoomType RoomType { get; set; }
    public bool AirConditioning { get; set; }
    public int MaxCapacity { get; set; }
    
    public ICollection<Bed> Beds { get; set; } = new List<Bed>();
    
    public string TenantId { get; set; } = string.Empty;
}
