using HostelHub.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HostelHub.Application.Features.Hostels.Queries;

public record GetHostelsCountQuery() : IRequest<int>;

public class GetHostelsCountQueryHandler : IRequestHandler<GetHostelsCountQuery, int>
{
    private readonly IUnitOfWork _unitOfWork;
    public GetHostelsCountQueryHandler(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;
    public async Task<int> Handle(GetHostelsCountQuery request, CancellationToken cancellationToken)
        => await _unitOfWork.Hostels.Entities.IgnoreQueryFilters().CountAsync(cancellationToken);
}
