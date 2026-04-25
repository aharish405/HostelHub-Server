using HostelHub.Domain.Entities;

namespace HostelHub.Application.Common.Interfaces;

public interface IUnitOfWork
{
    IRepository<Hostel> Hostels { get; }
    IRepository<Room> Rooms { get; }
    IRepository<Bed> Beds { get; }
    IRepository<Booking> Bookings { get; }
    IRepository<Payment> Payments { get; }
    IRepository<Guest> Guests { get; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
