using HostelHub.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HostelHub.Infrastructure.Persistence.Configurations;

public class PaymentConfiguration : IEntityTypeConfiguration<Payment>
{
    public void Configure(EntityTypeBuilder<Payment> builder)
    {
        builder.HasKey(p => p.Id);

        builder.Property(p => p.Amount)
            .HasPrecision(18, 2);
            
        builder.Property(p => p.Status)
            .IsRequired()
            .HasConversion<string>();
            
        builder.Property(p => p.TransactionId)
            .HasMaxLength(100);
            
        builder.Property(p => p.TenantId)
            .IsRequired()
            .HasMaxLength(50);
    }
}
