using HostelHub.Application.Common.Interfaces;
using HostelHub.Domain.Entities;
using HostelHub.Domain.Enums;
using MediatR;

namespace HostelHub.Application.Features.Hostels.Commands.OnboardHostel;

public class OnboardEnterpriseHostelCommandHandler : IRequestHandler<OnboardEnterpriseHostelCommand, string>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ITenantService _tenantService;

    public OnboardEnterpriseHostelCommandHandler(IUnitOfWork unitOfWork, ITenantService tenantService)
    {
        _unitOfWork = unitOfWork;
        _tenantService = tenantService;
    }

    public async Task<string> Handle(OnboardEnterpriseHostelCommand request, CancellationToken cancellationToken)
    {
        // Generate unique TenantID
        string newTenantId = $"{request.Name.Replace(" ", "").ToLowerInvariant()}-{Guid.NewGuid().ToString("N").Substring(0, 5)}";
        _tenantService.SetTenantId(newTenantId);

        var hostel = new Hostel
        {
            Name = request.Name,
            Street = request.Street,
            City = request.City,
            State = request.State,
            ZipCode = request.ZipCode,
            GSTIN = request.GSTIN,
            TenantId = newTenantId,
            Capacity = request.Floors.Sum(f => f.Rooms.Sum(r => r.BedCount))
        };

        foreach (var floorDto in request.Floors)
        {
            foreach (var roomDto in floorDto.Rooms)
            {
                var room = new Room
                {
                    RoomNumber = roomDto.RoomNumber,
                    FloorNumber = floorDto.FloorNumber,
                    RoomType = roomDto.RoomType,
                    AirConditioning = roomDto.HasAC,
                    MaxCapacity = roomDto.BedCount,
                    TenantId = newTenantId,
                    Hostel = hostel
                };

                for (int i = 1; i <= roomDto.BedCount; i++)
                {
                    room.Beds.Add(new Bed
                    {
                        BedName = $"{roomDto.RoomNumber}-{i}",
                        BedType = BedType.Single, // Default for now
                        Status = BedStatus.Available,
                        PricePerNight = roomDto.RoomType == RoomType.Private ? request.BasePrivatePrice : request.BaseDormPrice,
                        TenantId = newTenantId
                    });
                }
                hostel.Rooms.Add(room);
            }
        }

        await _unitOfWork.Hostels.AddAsync(hostel, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return newTenantId;
    }
}
