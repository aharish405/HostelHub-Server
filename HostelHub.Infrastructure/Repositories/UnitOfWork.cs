using HostelHub.Application.Common.Interfaces;
using HostelHub.Domain.Entities;
using HostelHub.Infrastructure.Persistence;

namespace HostelHub.Infrastructure.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private readonly ApplicationDbContext _context;
    private IRepository<Hostel>? _hostels;
    private IRepository<Room>? _rooms;
    private IRepository<Bed>? _beds;
    private IRepository<Booking>? _bookings;
    private IRepository<Payment>? _payments;
    private IRepository<Guest>? _guests;

    public UnitOfWork(ApplicationDbContext context)
    {
        _context = context;
    }

    public IRepository<Hostel> Hostels => _hostels ??= new Repository<Hostel>(_context);
    public IRepository<Room> Rooms => _rooms ??= new Repository<Room>(_context);
    public IRepository<Bed> Beds => _beds ??= new Repository<Bed>(_context);
    public IRepository<Booking> Bookings => _bookings ??= new Repository<Booking>(_context);
    public IRepository<Payment> Payments => _payments ??= new Repository<Payment>(_context);
    public IRepository<Guest> Guests => _guests ??= new Repository<Guest>(_context);

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await _context.SaveChangesAsync(cancellationToken);
    }
}
