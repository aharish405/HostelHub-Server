using HostelHub.Application.Common.Interfaces;
using HostelHub.Application.Features.Bookings.DTOs;
using HostelHub.Domain.Enums;
using MediatR;

namespace HostelHub.Application.Features.Bookings.Queries.GetAvailability;

public record GetAvailabilityQuery(
    Guid HostelId, 
    DateTime StartDate, 
    DateTime EndDate, 
    RoomType? RoomType) : IRequest<IEnumerable<AvailableBedDto>>;

public class GetAvailabilityQueryHandler : IRequestHandler<GetAvailabilityQuery, IEnumerable<AvailableBedDto>>
{
    private readonly IAvailabilityService _availabilityService;
    private readonly ITenantService _tenantService;

    public GetAvailabilityQueryHandler(IAvailabilityService availabilityService, ITenantService tenantService)
    {
        _availabilityService = availabilityService;
        _tenantService = tenantService;
    }

    public async Task<IEnumerable<AvailableBedDto>> Handle(GetAvailabilityQuery request, CancellationToken cancellationToken)
    {
        var tenantId = _tenantService.GetTenantId();
        if (string.IsNullOrEmpty(tenantId)) throw new UnauthorizedAccessException();

        return await _availabilityService.GetAvailableBedsAsync(
            request.HostelId, 
            request.StartDate, 
            request.EndDate, 
            request.RoomType, 
            tenantId);
    }
}
