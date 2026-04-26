using System;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using HostelHub.Application.Common.Interfaces;
using HostelHub.Domain.Entities;
using HostelHub.Domain.Enums;
using MediatR;

namespace HostelHub.Application.Features.Hostels.Commands.OnboardHostel;

public class OnboardHostelCommand : IRequest<string>
{
    public string Name { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public int Capacity { get; set; }
}

public class OnboardHostelCommandHandler : IRequestHandler<OnboardHostelCommand, string>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ITenantService _tenantService;

    public OnboardHostelCommandHandler(IUnitOfWork unitOfWork, ITenantService tenantService)
    {
        _unitOfWork = unitOfWork;
        _tenantService = tenantService;
    }

    public async Task<string> Handle(OnboardHostelCommand request, CancellationToken cancellationToken)
    {
        // 1. Generate unique TenantID
        string newTenantId = $"{request.Name.Replace(" ", "").ToLowerInvariant()}-{Guid.NewGuid().ToString("N").Substring(0, 5)}";

        // To ensure EF does not overwrite newly set TenantIds inside this unit of work step, 
        // we momentarily resolve the ITenantService state for the current async flow, or map it strictly.
        _tenantService.SetTenantId(newTenantId);

        // 2. Create Hostel Main Entity
        var hostel = new Hostel
        {
            Name = request.Name,
            Street = request.Address, // Legacy fallback
            Capacity = request.Capacity,
            TenantId = newTenantId
        };

        // 3. Seed Default Rooms
        var privateRoom = new Room
        {
            Hostel = hostel,
            RoomNumber = "PRV-01",
            RoomType = RoomType.Private,
            MaxCapacity = 2,
            TenantId = newTenantId
        };

        // Add beds to private room
        for (int i = 1; i <= 2; i++)
        {
            privateRoom.Beds.Add(new Bed { BedName = $"P{i}", Status = BedStatus.Available, PricePerNight = 50.0m, TenantId = newTenantId });
        }

        var dormRoom = new Room
        {
            Hostel = hostel,
            RoomNumber = "DRM-01",
            RoomType = RoomType.MixedDorm,
            MaxCapacity = 8,
            TenantId = newTenantId
        };

        // Add beds to dorm room
        for (int i = 1; i <= 8; i++)
        {
            dormRoom.Beds.Add(new Bed { BedName = $"D{i}", Status = BedStatus.Available, PricePerNight = 25.0m, TenantId = newTenantId });
        }
        
        hostel.Rooms.Add(privateRoom);
        hostel.Rooms.Add(dormRoom);

        await _unitOfWork.Hostels.AddAsync(hostel, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return newTenantId;
    }
}

public class OnboardHostelCommandValidator : AbstractValidator<OnboardHostelCommand>
{
    public OnboardHostelCommandValidator()
    {
        RuleFor(v => v.Name).NotEmpty().MaximumLength(150);
        RuleFor(v => v.Address).NotEmpty().MaximumLength(300);
        RuleFor(v => v.Capacity).GreaterThan(0);
    }
}
