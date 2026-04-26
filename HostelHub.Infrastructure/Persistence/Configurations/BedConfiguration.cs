using HostelHub.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HostelHub.Infrastructure.Persistence.Configurations;

public class BedConfiguration : IEntityTypeConfiguration<Bed>
{
    public void Configure(EntityTypeBuilder<Bed> builder)
    {
        builder.HasKey(b => b.Id);

        builder.Property(b => b.BedName)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(b => b.BedType)
            .IsRequired()
            .HasConversion<string>();
            
        builder.Property(b => b.Status)
            .IsRequired()
            .HasConversion<string>();
            
        builder.Property(b => b.PricePerNight)
            .HasPrecision(18, 2);
            
        builder.Property(b => b.TenantId)
            .IsRequired()
            .HasMaxLength(50);

        // Atomic tracking to prevent double-booking
        builder.Property(b => b.Version)
            .IsRowVersion();
    }
}
