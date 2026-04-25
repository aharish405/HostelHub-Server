using HostelHub.Application.Common.Interfaces;
using HostelHub.Domain.Constants;
using HostelHub.Domain.Entities;
using HostelHub.Domain.Enums;
using HostelHub.Infrastructure.Identity;
using HostelHub.Infrastructure.Persistence;
using HostelHub.Tests.Integration.Common;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using HostelHub.Application.Features.Bookings.Commands.CreateBooking;

namespace HostelHub.Tests.Integration.Features.Bookings;

public class BookingConcurrencyTests(IntegrationTestFactory factory) : BaseIntegrationTest(factory)
{
    [Fact]
    public async Task SimultaneousBookings_OnlyOneSucceeds()
    {
        // 1. Arrange
        using var scope = Factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
        var jwtService = scope.ServiceProvider.GetRequiredService<IJwtService>();

        await roleManager.CreateAsync(new IdentityRole(Roles.HostelAdmin));
        
        var tenantId = "tenant-race";
        var hostel = new Hostel { Name = "Race Hostel", Address = "Race 1", TenantId = tenantId };
        var room = new Room { Hostel = hostel, RoomNumber = "R1", RoomType = RoomType.Dormitory, TenantId = tenantId };
        var bed = new Bed { Room = room, BedNumber = "B1", Status = BedStatus.Available, PricePerNight = 10, TenantId = tenantId };
        context.Hostels.Add(hostel);
        await context.SaveChangesAsync();

        var adminUser = new ApplicationUser { UserName = "race@test.com", Email = "race@test.com", TenantId = tenantId };
        await userManager.CreateAsync(adminUser, "Password123!");
        await userManager.AddToRoleAsync(adminUser, Roles.HostelAdmin);
        var token = await jwtService.GenerateTokenAsync(adminUser.Id, adminUser.Email, new[] { Roles.HostelAdmin }, tenantId);

        AuthenticateAs(token, tenantId);

        // 2. Act: Send 10 simultaneous booking requests
        var tasks = new List<Task<HttpResponseMessage>>();
        var command = new CreateBookingCommand(
            bed.Id, 
            DateTime.Today.AddDays(5), 
            DateTime.Today.AddDays(7), 
            Guid.NewGuid());

        for (int i = 0; i < 10; i++)
        {
            tasks.Add(Client.PostAsJsonAsync("/api/v1/Bookings", command));
        }

        var responses = await Task.WhenAll(tasks);

        // 3. Assert
        var successCount = responses.Count(r => r.IsSuccessStatusCode);
        var failureCount = responses.Count(r => !r.IsSuccessStatusCode);

        successCount.Should().Be(1, "Only one booking should succeed for the same bed and dates");
        failureCount.Should().Be(9, "Nine bookings should fail due to unavailability or concurrency");
    }
}
