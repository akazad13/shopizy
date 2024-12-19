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
        builder.ToTable("Payments");
        builder.HasKey(p => p.Id);

        builder
            .Property(p => p.Id)
            .ValueGeneratedNever()
            .HasConversion(id => id.Value, value => PaymentId.Create(value));

        builder.Property(p => p.PaymentMethod).HasMaxLength(20);
        builder.Property(p => p.PaymentMethodId).HasMaxLength(260);
        builder.Property(p => p.TransactionId).HasMaxLength(260).IsRequired(false);
        builder.Property(p => p.PaymentStatus);
        builder.Property(p => p.CreatedOn).HasColumnType("smalldatetime");
        builder.Property(p => p.ModifiedOn).HasColumnType("smalldatetime").IsRequired(false);

        builder.OwnsOne(
            p => p.Total,
            pb =>
            {
                pb.Property(p => p.Amount).HasPrecision(18, 2);
            }
        );
        builder.OwnsOne(
            p => p.BillingAddress,
            ab =>
            {
                ab.Property(ad => ad.Street).HasMaxLength(100);
                ab.Property(ad => ad.City).HasMaxLength(30);
                ab.Property(ad => ad.State).HasMaxLength(30);
                ab.Property(ad => ad.Country).HasMaxLength(30);
                ab.Property(ad => ad.ZipCode).HasMaxLength(10);
            }
        );
        builder
            .Property(p => p.UserId)
            .HasConversion(id => id.Value, value => UserId.Create(value));
        builder
            .Property(p => p.OrderId)
            .HasConversion(id => id.Value, value => OrderId.Create(value));

        builder.HasOne(c => c.User).WithMany().OnDelete(DeleteBehavior.NoAction);
        builder.HasOne(c => c.Order).WithMany().OnDelete(DeleteBehavior.NoAction);
    }
}
