using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Shopizy.Domain.Orders.ValueObjects;
using Shopizy.Domain.Payments;
using Shopizy.Domain.Payments.ValueObjects;
using Shopizy.Domain.Users.ValueObjects;

namespace Shopizy.Infrastructure.Payments.Persistence;

public sealed class PaymentConfigurations : IEntityTypeConfiguration<Payment>
{
    public void Configure(EntityTypeBuilder<Payment> builder)
    {
        ConfigurePaymentsTable(builder);
    }

    private static void ConfigurePaymentsTable(EntityTypeBuilder<Payment> builder)
    {
        _ = builder.ToTable("Payments");
        _ = builder.HasKey(p => p.Id);

        _ = builder
            .Property(p => p.Id)
            .ValueGeneratedNever()
            .HasConversion(id => id.Value, value => PaymentId.Create(value));

        _ = builder.Property(p => p.PaymentMethod).HasMaxLength(20);
        _ = builder.Property(p => p.TransactionId).HasMaxLength(50).IsRequired(false);
        _ = builder.Property(p => p.PaymentStatus);
        _ = builder.Property(p => p.CreatedOn).HasColumnType("smalldatetime");
        _ = builder.Property(p => p.ModifiedOn).HasColumnType("smalldatetime").IsRequired(false);

        _ = builder.OwnsOne(
            p => p.Total,
            pb =>
            {
                _ = pb.Property(p => p.Amount).HasPrecision(18, 2);
            }
        );
        _ = builder.OwnsOne(
            p => p.BillingAddress,
            ab =>
            {
                _ = ab.Property(ad => ad.Street).HasMaxLength(100);
                _ = ab.Property(ad => ad.City).HasMaxLength(30);
                _ = ab.Property(ad => ad.State).HasMaxLength(30);
                _ = ab.Property(ad => ad.Country).HasMaxLength(30);
                _ = ab.Property(ad => ad.ZipCode).HasMaxLength(10);
            }
        );
        _ = builder
            .Property(p => p.UserId)
            .HasConversion(id => id.Value, value => UserId.Create(value));
        _ = builder
            .Property(p => p.OrderId)
            .HasConversion(id => id.Value, value => OrderId.Create(value));

        _ = builder.HasOne(c => c.User).WithOne().OnDelete(DeleteBehavior.NoAction);
        _ = builder.HasOne(c => c.Order).WithOne().OnDelete(DeleteBehavior.NoAction);
    }
}
