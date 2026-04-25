using System.Data;
using Dapper;
using HostelHub.Application.Common.Interfaces;
using HostelHub.Application.Features.Bookings.DTOs;
using HostelHub.Domain.Enums;
using HostelHub.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace HostelHub.Infrastructure.Services;

public class AvailabilityService : IAvailabilityService
{
    private readonly ApplicationDbContext _context;

    public AvailabilityService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<AvailableBedDto>> GetAvailableBedsAsync(
        Guid hostelId, 
        DateTime startDate, 
        DateTime endDate, 
        RoomType? roomType, 
        string tenantId)
    {
        using var connection = _context.Database.GetDbConnection();
        
        const string sql = @"
            SELECT 
                b.Id as BedId, 
                b.BedNumber, 
                b.RoomId, 
                r.RoomNumber, 
                r.RoomType, 
                b.PricePerNight
            FROM Beds b
            JOIN Rooms r ON b.RoomId = r.Id
            WHERE r.HostelId = @HostelId
            AND b.TenantId = @TenantId
            AND b.Status = 'Available'
            AND (@RoomType IS NULL OR r.RoomType = @RoomType)
            AND NOT EXISTS (
                SELECT 1 FROM Bookings bk
                JOIN Beds b2 ON bk.BedId = b2.Id
                WHERE (
                    -- Dormitory: Check specific bed
                    (r.RoomType = 'Dormitory' AND bk.BedId = b.Id)
                    OR
                    -- Private: Check ANY bed in the room
                    (r.RoomType = 'Private' AND b2.RoomId = r.Id)
                )
                AND bk.Status != 'Cancelled'
                AND bk.StartDate < @EndDate 
                AND bk.EndDate > @StartDate
            )";

        var parameters = new
        {
            HostelId = hostelId,
            TenantId = tenantId,
            StartDate = startDate,
            EndDate = endDate,
            RoomType = roomType?.ToString() // Since we use string conversion in EF config
        };

        return await connection.QueryAsync<AvailableBedDto>(sql, parameters);
    }
}
