using HostelHub.Application.Common.Interfaces;
using HostelHub.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HostelHub.Application.Features.Beds.Queries;

public record GetTotalBedsQuery() : IRequest<IEnumerable<Bed>>;

public class GetTotalBedsQueryHandler : IRequestHandler<GetTotalBedsQuery, IEnumerable<Bed>>
{
    private readonly IUnitOfWork _unitOfWork;
    public GetTotalBedsQueryHandler(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;
    public async Task<IEnumerable<Bed>> Handle(GetTotalBedsQuery request, CancellationToken cancellationToken)
        => await _unitOfWork.Beds.Entities.IgnoreQueryFilters().ToListAsync(cancellationToken);
}
