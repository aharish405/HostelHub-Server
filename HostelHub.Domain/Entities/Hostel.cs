using HostelHub.Domain.Common;

namespace HostelHub.Domain.Entities;

public class Hostel : BaseEntity, IMustHaveTenant
{
    public string Name { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public int Capacity { get; set; }
    
    public ICollection<Room> Rooms { get; set; } = new List<Room>();
    
    public string TenantId { get; set; } = string.Empty;
}
