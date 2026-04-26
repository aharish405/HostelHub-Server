using HostelHub.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HostelHub.Application.Features.Beds.Queries;

public record GetTotalBedsCountQuery() : IRequest<int>;

public class GetTotalBedsCountQueryHandler : IRequestHandler<GetTotalBedsCountQuery, int>
{
    private readonly IUnitOfWork _unitOfWork;
    public GetTotalBedsCountQueryHandler(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;
    public async Task<int> Handle(GetTotalBedsCountQuery request, CancellationToken cancellationToken)
        => await _unitOfWork.Beds.Entities.IgnoreQueryFilters().CountAsync(cancellationToken);
}
