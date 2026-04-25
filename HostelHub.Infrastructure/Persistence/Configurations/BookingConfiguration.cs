using HostelHub.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HostelHub.Infrastructure.Persistence.Configurations;

public class BookingConfiguration : IEntityTypeConfiguration<Booking>
{
    public void Configure(EntityTypeBuilder<Booking> builder)
    {
        builder.HasKey(b => b.Id);

        builder.Property(b => b.Status)
            .IsRequired()
            .HasConversion<string>();
            
        builder.Property(b => b.TenantId)
            .IsRequired()
            .HasMaxLength(50);

        builder.HasOne(b => b.Bed)
            .WithMany()
            .HasForeignKey(b => b.BedId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(b => b.Payments)
            .WithOne(p => p.Booking)
            .HasForeignKey(p => p.BookingId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
