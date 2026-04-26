using HostelHub.Application.Common.Interfaces;
using HostelHub.Domain.Common;
using HostelHub.Domain.Entities;
using HostelHub.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace HostelHub.Infrastructure.Persistence;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
    private readonly ITenantService _tenantService;

    public ApplicationDbContext(
        DbContextOptions<ApplicationDbContext> options,
        ITenantService tenantService) : base(options)
    {
        _tenantService = tenantService;
    }

    public DbSet<Hostel> Hostels { get; set; }
    public DbSet<Room> Rooms { get; set; }
    public DbSet<Bed> Beds { get; set; }
    public DbSet<Booking> Bookings { get; set; }
    public DbSet<Payment> Payments { get; set; }
    public DbSet<Guest> Guests { get; set; }

    private string? CurrentTenantId => _tenantService.GetTenantId();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

        // Apply Global Query Filter for Multi-tenancy
        // IMPORTANT: EF Core captures this expression. We must reference the property/field 
        // of the context so it evaluates correctly at execution time.
        builder.Entity<Hostel>().HasQueryFilter(e => e.TenantId == CurrentTenantId);
        builder.Entity<Room>().HasQueryFilter(e => e.TenantId == CurrentTenantId);
        builder.Entity<Bed>().HasQueryFilter(e => e.TenantId == CurrentTenantId);
        builder.Entity<Booking>().HasQueryFilter(e => e.TenantId == CurrentTenantId);
        builder.Entity<Payment>().HasQueryFilter(e => e.TenantId == CurrentTenantId);
        builder.Entity<Guest>().HasQueryFilter(e => e.TenantId == CurrentTenantId);
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var tenantId = _tenantService.GetTenantId();
        
        foreach (var entry in ChangeTracker.Entries<IMustHaveTenant>())
        {
            switch (entry.State)
            {
                case EntityState.Added:
                case EntityState.Modified:
                    if (string.IsNullOrEmpty(entry.Entity.TenantId))
                    {
                        if (string.IsNullOrEmpty(tenantId))
                            throw new Exception("TenantId must be provided for this entity.");
                        
                        entry.Entity.TenantId = tenantId;
                    }
                    break;
            }
        }

        foreach (var entry in ChangeTracker.Entries<BaseEntity>())
        {
            switch (entry.State)
            {
                case EntityState.Added:
                    entry.Entity.CreatedAt = DateTime.UtcNow;
                    break;
                case EntityState.Modified:
                    entry.Entity.UpdatedAt = DateTime.UtcNow;
                    break;
            }
        }

        return base.SaveChangesAsync(cancellationToken);
    }
}
