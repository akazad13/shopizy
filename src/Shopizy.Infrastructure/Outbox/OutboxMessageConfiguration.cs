using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Shopizy.Infrastructure.Outbox;

public class OutboxMessageConfiguration : IEntityTypeConfiguration<OutboxMessage>
{
    public void Configure(EntityTypeBuilder<OutboxMessage> builder)
    {
        builder.ToTable("OutboxMessages");
        builder.HasKey(m => m.Id);
        builder.Property(m => m.Type).HasMaxLength(500).IsRequired();
        builder.Property(m => m.Content).HasColumnType("nvarchar(max)").IsRequired();
        builder.Property(m => m.DeadLetterReason).HasMaxLength(1000);
        builder.HasIndex(m => m.ProcessedOn);
        builder.HasIndex(m => m.DeadLetteredOn);
    }
}
