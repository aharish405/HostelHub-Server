using HostelHub.Domain.Enums;
using MediatR;

namespace HostelHub.Application.Features.Hostels.Commands.OnboardHostel;

public class OnboardEnterpriseHostelCommand : IRequest<string>
{
    // Identity & Tax
    public string Name { get; set; } = string.Empty;
    public string Street { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string State { get; set; } = string.Empty;
    public string ZipCode { get; set; } = string.Empty;
    public string GSTIN { get; set; } = string.Empty;
    
    // Configuration
    public List<FloorConfigurationDto> Floors { get; set; } = new();
    
    // Pricing Defaults
    public decimal BasePrivatePrice { get; set; }
    public decimal BaseDormPrice { get; set; }
}

public class FloorConfigurationDto
{
    public int FloorNumber { get; set; }
    public List<RoomConfigurationDto> Rooms { get; set; } = new();
}

public class RoomConfigurationDto
{
    public string RoomNumber { get; set; } = string.Empty;
    public RoomType RoomType { get; set; }
    public bool HasAC { get; set; }
    public int BedCount { get; set; }
}
