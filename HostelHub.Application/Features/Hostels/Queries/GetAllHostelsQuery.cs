using HostelHub.Application.Common.Interfaces;
using HostelHub.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HostelHub.Application.Features.Hostels.Queries;

public record GetAllHostelsQuery() : IRequest<IEnumerable<Hostel>>;

public class GetAllHostelsQueryHandler : IRequestHandler<GetAllHostelsQuery, IEnumerable<Hostel>>
{
    private readonly IUnitOfWork _unitOfWork;
    public GetAllHostelsQueryHandler(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;
    public async Task<IEnumerable<Hostel>> Handle(GetAllHostelsQuery request, CancellationToken cancellationToken)
        => await _unitOfWork.Hostels.Entities.IgnoreQueryFilters().ToListAsync(cancellationToken);
}
