using AutoMapper;
using HostelHub.Application.Features.Hostels.Commands.CreateHostel;
using HostelHub.Application.Features.Hostels.Commands.OnboardHostel;
using HostelHub.Domain.Entities;

namespace HostelHub.Application.Common.Mappings;

public class HostelProfile : Profile
{
    public HostelProfile()
    {
        // Add mappings here as logic expands
        CreateMap<CreateHostelCommand, Hostel>();
        CreateMap<OnboardHostelCommand, Hostel>();
    }
}
