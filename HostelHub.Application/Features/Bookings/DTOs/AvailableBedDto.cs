using HostelHub.Domain.Enums;

namespace HostelHub.Application.Features.Bookings.DTOs;

public class AvailableBedDto
{
    public Guid BedId { get; set; }
    public string BedNumber { get; set; } = string.Empty;
    public Guid RoomId { get; set; }
    public string RoomNumber { get; set; } = string.Empty;
    public RoomType RoomType { get; set; }
    public decimal PricePerNight { get; set; }
}
