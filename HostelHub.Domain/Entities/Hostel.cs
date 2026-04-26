using HostelHub.Domain.Common;

namespace HostelHub.Domain.Entities;

public class Hostel : BaseEntity, IMustHaveTenant
{
    public string Name { get; set; } = string.Empty;
    public string Street { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string State { get; set; } = string.Empty;
    public string ZipCode { get; set; } = string.Empty;
    
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    
    public string GSTIN { get; set; } = string.Empty;
    public string CheckInPolicy { get; set; } = "14:00";
    public string CheckOutPolicy { get; set; } = "11:00";
    
    public List<string> Amenities { get; set; } = new();
    
    public int Capacity { get; set; }
    
    public ICollection<Room> Rooms { get; set; } = new List<Room>();
    
    public string TenantId { get; set; } = string.Empty;
}
