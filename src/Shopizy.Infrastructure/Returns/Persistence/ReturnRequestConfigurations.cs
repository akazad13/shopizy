using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Shopizy.Domain.Orders.ValueObjects;
using Shopizy.Domain.Returns;
using Shopizy.Domain.Returns.Entities;
using Shopizy.Domain.Returns.ValueObjects;
using Shopizy.Domain.Users.ValueObjects;

namespace Shopizy.Infrastructure.Returns.Persistence;

public sealed class ReturnRequestConfigurations : IEntityTypeConfiguration<ReturnRequest>
{
    public void Configure(EntityTypeBuilder<ReturnRequest> builder)
    {
        builder.ToTable("ReturnRequests");
        builder.HasKey(r => r.Id);

        builder
            .Property(r => r.Id)
            .ValueGeneratedNever()
            .HasConversion(id => id.Value, value => ReturnRequestId.Create(value));

        builder
            .Property(r => r.OrderId)
            .HasConversion(id => id.Value, value => OrderId.Create(value));

        builder
            .Property(r => r.UserId)
            .HasConversion(id => id.Value, value => UserId.Create(value));

        builder.Property(r => r.Reason).HasMaxLength(500);
        builder.Property(r => r.AdminNote).HasMaxLength(500).IsRequired(false);
        builder.Property(r => r.Status).HasConversion<int>();
        builder.Property(r => r.CreatedOn).HasColumnType("smalldatetime");
        builder.Property(r => r.ModifiedOn).HasColumnType("smalldatetime").IsRequired(false);

        builder.HasIndex(r => r.OrderId);
        builder.HasIndex(r => r.UserId);
        builder.HasIndex(r => r.Status);

        builder.Property<byte[]>("RowVersion");

        builder.OwnsMany(
            r => r.Items,
            ib =>
            {
                ib.ToTable("ReturnItems");
                ib.WithOwner().HasForeignKey("ReturnRequestId");
                ib.HasKey(nameof(ReturnItem.Id), "ReturnRequestId");

                ib.Property(i => i.Id)
                    .ValueGeneratedNever()
                    .HasConversion(id => id.Value, value => ReturnItemId.Create(value));

                ib.Property(i => i.OrderItemId)
                    .HasConversion(id => id.Value, value => OrderItemId.Create(value));

                ib.Property(i => i.Quantity);
            }
        );

        builder.Navigation(r => r.Items).UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}
