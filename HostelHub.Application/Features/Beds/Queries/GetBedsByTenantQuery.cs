using HostelHub.Application.Common.Interfaces;
using HostelHub.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HostelHub.Application.Features.Beds.Queries;

public record GetBedsByTenantQuery() : IRequest<IEnumerable<Bed>>;

public class GetBedsByTenantQueryHandler : IRequestHandler<GetBedsByTenantQuery, IEnumerable<Bed>>
{
    private readonly IUnitOfWork _unitOfWork;
    public GetBedsByTenantQueryHandler(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;
    public async Task<IEnumerable<Bed>> Handle(GetBedsByTenantQuery request, CancellationToken cancellationToken)
        => await _unitOfWork.Beds.Entities.ToListAsync(cancellationToken);
}
