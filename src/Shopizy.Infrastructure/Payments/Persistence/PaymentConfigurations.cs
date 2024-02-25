using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Shopizy.Domain.Customers.ValueObject;
using Shopizy.Domain.Orders.ValueObjects;
using Shopizy.Domain.Payments;
using Shopizy.Domain.Payments.ValueObjects;

namespace shopizy.Infrastructure.Payments.Persistence;

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
        builder.Property(p => p.TransactionId).HasMaxLength(50);
        builder.Property(p => p.PaymentStatus);
        builder.Property(p => p.CreatedOn);
        builder.Property(p => p.ModifiedOn);

        builder.OwnsOne(p => p.Total);
        builder.OwnsOne(p => p.BillingAddress);
        builder
            .Property(p => p.CustomerId)
            .HasConversion(id => id.Value, value => CustomerId.Create(value));
        builder
            .Property(p => p.OrderId)
            .HasConversion(id => id.Value, value => OrderId.Create(value));
    }
}
