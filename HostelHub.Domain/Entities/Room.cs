using HostelHub.Domain.Common;
using HostelHub.Domain.Enums;

namespace HostelHub.Domain.Entities;

public class Room : BaseEntity, IMustHaveTenant
{
    public Guid HostelId { get; set; }
    public Hostel Hostel { get; set; } = null!;
    
    public string RoomNumber { get; set; } = string.Empty;
    public RoomType RoomType { get; set; }
    public int MaxCapacity { get; set; }
    
    public ICollection<Bed> Beds { get; set; } = new List<Bed>();
    
    public string TenantId { get; set; } = string.Empty;
}
