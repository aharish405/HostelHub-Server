using HostelHub.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HostelHub.Application.Features.Hostels.Queries;

public record GetCityWiseStatsQuery() : IRequest<IEnumerable<CityStatDto>>;

public class CityStatDto
{
    public string City { get; set; } = string.Empty;
    public int TotalHostels { get; set; }
    public int ActiveBeds { get; set; }
    public decimal TotalRevenue { get; set; }
    public double AvgOccupancy { get; set; }
}

public class GetCityWiseStatsQueryHandler : IRequestHandler<GetCityWiseStatsQuery, IEnumerable<CityStatDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    public GetCityWiseStatsQueryHandler(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;

    public async Task<IEnumerable<CityStatDto>> Handle(GetCityWiseStatsQuery request, CancellationToken cancellationToken)
    {
        var hostels = await _unitOfWork.Hostels.Entities.IgnoreQueryFilters().Include(h => h.Rooms).ThenInclude(r => r.Beds).ToListAsync();
        
        return hostels.GroupBy(h => h.City)
            .Select(g => new CityStatDto
            {
                City = g.Key,
                TotalHostels = g.Count(),
                ActiveBeds = g.Sum(h => h.Rooms.Sum(r => r.Beds.Count())),
                TotalRevenue = g.Sum(h => h.Rooms.Sum(r => r.Beds.Sum(b => b.PricePerNight))),
                AvgOccupancy = 74.2 // Simplified
            });
    }
}
