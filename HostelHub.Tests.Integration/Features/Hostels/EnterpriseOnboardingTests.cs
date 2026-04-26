using HostelHub.Application.Features.Hostels.Commands.OnboardHostel;
using HostelHub.Tests.Integration.Common;
using System.Net.Http.Json;
using FluentAssertions;
using HostelHub.Domain.Enums;
using HostelHub.Domain.Constants;
using Microsoft.Extensions.DependencyInjection;
using HostelHub.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace HostelHub.Tests.Integration.Features.Hostels;

public class EnterpriseOnboardingTests(IntegrationTestFactory factory) : BaseIntegrationTest(factory)
{
    [Fact]
    public async Task BulkOnboarding_ShouldCreate_50PlusBedsAtomically()
    {
        // 1. Arrange: Authenticate as SuperAdmin
        AuthenticateAsAdmin();

        var command = new OnboardEnterpriseHostelCommand
        {
            Name = "Mega Hostel",
            Street = "Tech Park Road",
            City = "Hyderabad",
            State = "Telangana",
            ZipCode = "500081",
            GSTIN = "36AAAAA0000A1Z5",
            BasePrivatePrice = 1200,
            BaseDormPrice = 450,
            Floors = new List<FloorConfigurationDto>()
        };

        // Define 2 floors with total 60 beds
        for (int f = 0; f < 2; f++)
        {
            var floor = new FloorConfigurationDto { FloorNumber = f, Rooms = new List<RoomConfigurationDto>() };
            for (int r = 1; r <= 5; r++)
            {
                floor.Rooms.Add(new RoomConfigurationDto 
                { 
                    RoomNumber = $"{f}{r:00}", 
                    RoomType = RoomType.MixedDorm, 
                    HasAC = true, 
                    BedCount = 6 // 6 beds per room * 5 rooms * 2 floors = 60 beds
                });
            }
            command.Floors.Add(floor);
        }

        // 2. Act
        var response = await Client.PostAsJsonAsync("/api/v1/Hostels/onboard/enterprise", command);

        // 3. Assert
        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadFromJsonAsync<JsonElement>();
        var tenantId = result.GetProperty("tenantId").GetString();

        // Verify in DB
        using var scope = Factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        
        var bedCount = await context.Beds.IgnoreQueryFilters()
            .Where(b => b.TenantId == tenantId)
            .CountAsync();

        bedCount.Should().Be(60, "Bulk onboarding should create exactly 60 beds across the defined hierarchy.");
    }
}
