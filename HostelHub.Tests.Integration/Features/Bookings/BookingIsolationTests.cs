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
using System.Text.Json;
using FluentAssertions;

namespace HostelHub.Tests.Integration.Features.Bookings;

public class BookingIsolationTests(IntegrationTestFactory factory) : BaseIntegrationTest(factory)
{
    [Fact]
    public async Task AdminB_CannotAccess_BookingInHostelA()
    {
        // 1. Arrange: Seed Hostels, Users, and a Booking
        using var scope = Factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
        var jwtService = scope.ServiceProvider.GetRequiredService<IJwtService>();

        // Ensure roles exist
        await roleManager.CreateAsync(new IdentityRole(Roles.HostelAdmin));

        // Create Hostel A
        var hostelA = new Hostel { Name = "Hostel A", Address = "Address A", TenantId = "tenant-a" };
        var roomA = new Room { Hostel = hostelA, RoomNumber = "A1", RoomType = RoomType.Dormitory, TenantId = "tenant-a" };
        var bedA = new Bed { Room = roomA, BedNumber = "B1", Status = BedStatus.Available, PricePerNight = 10, TenantId = "tenant-a" };
        context.Hostels.Add(hostelA);
        await context.SaveChangesAsync();

        // Create Admin A
        var adminAUser = new ApplicationUser { UserName = "admina@test.com", Email = "admina@test.com", TenantId = "tenant-a" };
        await userManager.CreateAsync(adminAUser, "Password123!");
        await userManager.AddToRoleAsync(adminAUser, Roles.HostelAdmin);
        var tokenA = await jwtService.GenerateTokenAsync(adminAUser.Id, adminAUser.Email, new[] { Roles.HostelAdmin }, "tenant-a");

        // Create Booking in Hostel A
        var bookingA = new Booking 
        { 
            BedId = bedA.Id, 
            GuestId = Guid.NewGuid(), 
            StartDate = DateTime.UtcNow.AddDays(1), 
            EndDate = DateTime.UtcNow.AddDays(3), 
            Status = BookingStatus.Confirmed,
            TenantId = "tenant-a" 
        };
        context.Bookings.Add(bookingA);
        await context.SaveChangesAsync();

        // Create Admin B
        var hostelB = new Hostel { Name = "Hostel B", Address = "Address B", TenantId = "tenant-b" };
        context.Hostels.Add(hostelB);
        await context.SaveChangesAsync();
        
        var adminBUser = new ApplicationUser { UserName = "adminb@test.com", Email = "adminb@test.com", TenantId = "tenant-b" };
        await userManager.CreateAsync(adminBUser, "Password123!");
        await userManager.AddToRoleAsync(adminBUser, Roles.HostelAdmin);
        var tokenB = await jwtService.GenerateTokenAsync(adminBUser.Id, adminBUser.Email, new[] { Roles.HostelAdmin }, "tenant-b");

        // 2. Act: Admin B tries to Book or Check Availability using Tenant A ID
        // Note: Our [Authorize(Policy = "TenantLocked")] checks if claim TenantId matches X-Tenant-ID header.
        // If Admin B sends X-Tenant-ID: tenant-a, the policy should fail (403).
        AuthenticateAs(tokenB, "tenant-a");

        var response = await Client.GetAsync($"/api/v1/Bookings/availability?hostelId={hostelA.Id}&startDate={DateTime.UtcNow.AddDays(1):O}&endDate={DateTime.UtcNow.AddDays(2):O}");

        // 3. Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }
}
