using HostelHub.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HostelHub.Infrastructure.Persistence.Configurations;

public class HostelConfiguration : IEntityTypeConfiguration<Hostel>
{
    public void Configure(EntityTypeBuilder<Hostel> builder)
    {
        builder.HasKey(h => h.Id);
        
        builder.Property(h => h.Name)
            .IsRequired()
            .HasMaxLength(150);
            
        builder.Property(h => h.Address)
            .IsRequired()
            .HasMaxLength(300);
            
        builder.Property(h => h.TenantId)
            .IsRequired()
            .HasMaxLength(50);
            
        builder.HasMany(h => h.Rooms)
            .WithOne(r => r.Hostel)
            .HasForeignKey(r => r.HostelId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
