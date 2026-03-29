using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Shopizy.Domain.AuditLogs;
using Shopizy.Domain.AuditLogs.ValueObjects;

namespace Shopizy.Infrastructure.AuditLogs.Persistence;

public sealed class AuditLogConfigurations : IEntityTypeConfiguration<AuditLog>
{
    public void Configure(EntityTypeBuilder<AuditLog> builder)
    {
        ConfigureAuditLogsTable(builder);
    }

    private static void ConfigureAuditLogsTable(EntityTypeBuilder<AuditLog> builder)
    {
        builder.ToTable("AuditLogs");
        builder.HasKey(al => al.Id);

        builder
            .Property(al => al.Id)
            .ValueGeneratedNever()
            .HasConversion(id => id.Value, value => AuditLogId.Create(value));

        builder.Property(al => al.Action).HasMaxLength(100).IsRequired();
        builder.Property(al => al.EntityName).HasMaxLength(100).IsRequired();
        builder.Property(al => al.EntityId).HasMaxLength(100).IsRequired();
        builder.Property(al => al.OldValues).IsRequired(false);
        builder.Property(al => al.NewValues).IsRequired(false);
        builder.Property(al => al.Timestamp).HasColumnType("smalldatetime");
        builder.Property(al => al.UserId).IsRequired(false);
    }
}
