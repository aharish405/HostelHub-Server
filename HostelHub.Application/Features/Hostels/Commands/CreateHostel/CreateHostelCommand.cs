using FluentValidation;
using HostelHub.Application.Common.Interfaces;
using HostelHub.Domain.Entities;
using MediatR;

namespace HostelHub.Application.Features.Hostels.Commands.CreateHostel;

public record CreateHostelCommand(string Name, string Address, int Capacity) : IRequest<Guid>;

public class CreateHostelCommandHandler : IRequestHandler<CreateHostelCommand, Guid>
{
    private readonly IUnitOfWork _unitOfWork;

    public CreateHostelCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Guid> Handle(CreateHostelCommand request, CancellationToken cancellationToken)
    {
        var hostel = new Hostel
        {
            Name = request.Name,
            Address = request.Address,
            Capacity = request.Capacity
        };

        await _unitOfWork.Hostels.AddAsync(hostel, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return hostel.Id;
    }
}

public class CreateHostelCommandValidator : AbstractValidator<CreateHostelCommand>
{
    public CreateHostelCommandValidator()
    {
        RuleFor(v => v.Name)
            .MaximumLength(100)
            .NotEmpty();

        RuleFor(v => v.Address)
            .MaximumLength(200)
            .NotEmpty();

        RuleFor(v => v.Capacity)
            .GreaterThan(0);
    }
}
