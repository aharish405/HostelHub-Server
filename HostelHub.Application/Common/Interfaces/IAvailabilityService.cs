using HostelHub.Application.Features.Bookings.DTOs;
using HostelHub.Domain.Enums;

namespace HostelHub.Application.Common.Interfaces;

public interface IAvailabilityService
{
    Task<IEnumerable<AvailableBedDto>> GetAvailableBedsAsync(
        Guid hostelId, 
        DateTime startDate, 
        DateTime endDate, 
        RoomType? roomType, 
        string tenantId);
}
