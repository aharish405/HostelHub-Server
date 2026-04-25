using HostelHub.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HostelHub.Infrastructure.Persistence.Configurations;

public class RoomConfiguration : IEntityTypeConfiguration<Room>
{
    public void Configure(EntityTypeBuilder<Room> builder)
    {
        builder.HasKey(r => r.Id);

        builder.Property(r => r.RoomNumber)
            .IsRequired()
            .HasMaxLength(50);
            
        builder.Property(r => r.RoomType)
            .IsRequired()
            .HasConversion<string>(); // Store enum as string
            
        builder.Property(r => r.TenantId)
            .IsRequired()
            .HasMaxLength(50);
            
        builder.HasMany(r => r.Beds)
            .WithOne(b => b.Room)
            .HasForeignKey(b => b.RoomId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
