using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Shopizy.Domain.Customers.ValueObject;
using Shopizy.Domain.Orders;
using Shopizy.Domain.Orders.Entities;
using Shopizy.Domain.Orders.ValueObjects;

namespace shopizy.Infrastructure.Orders.Persistence;

public sealed class OrderConfigurations : IEntityTypeConfiguration<Order>
{
    public void Configure(EntityTypeBuilder<Order> builder)
    {
        ConfigureOrdersTable(builder);
        ConfigureOrderItemsTable(builder);
    }

    private static void ConfigureOrdersTable(EntityTypeBuilder<Order> builder)
    {
        builder.ToTable("Orders");
        builder.HasKey(c => c.Id);

        builder
            .Property(c => c.Id)
            .ValueGeneratedNever()
            .HasConversion(id => id.Value, value => OrderId.Create(value));

        builder.Property(o => o.PromoCode).HasMaxLength(15);
        builder.Property(o => o.CreatedOn).HasColumnType("smalldatetime");
        builder.Property(o => o.ModifiedOn).HasColumnType("smalldatetime");
        builder.Property(o => o.OrderStatus);
        builder.Property(o => o.PaymentStatus).HasMaxLength(20);

        builder.OwnsOne(
            o => o.DeliveryCharge,
            pb =>
            {
                pb.Property(p => p.Amount).HasPrecision(18, 2);
            }
        );
        builder.OwnsOne(
            o => o.ShippingAddress,
            ab =>
            {
                ab.Property(ad => ad.Line).HasMaxLength(100);
                ab.Property(ad => ad.City).HasMaxLength(30);
                ab.Property(ad => ad.State).HasMaxLength(30);
                ab.Property(ad => ad.Country).HasMaxLength(30);
                ab.Property(ad => ad.ZipCode).HasMaxLength(10);
            }
        );
        builder
            .Property(c => c.CustomerId)
            .HasConversion(id => id.Value, value => CustomerId.Create(value));
    }

    private static void ConfigureOrderItemsTable(EntityTypeBuilder<Order> builder)
    {
        builder.OwnsMany(
            o => o.OrderItems,
            ib =>
            {
                ib.ToTable("OrderItems");
                ib.WithOwner().HasForeignKey("OrderId");
                ib.HasKey(nameof(OrderItem.Id), "OrderId");

                ib.Property(oi => oi.Id)
                    .ValueGeneratedNever()
                    .HasConversion(id => id.Value, value => OrderItemId.Create(value));

                ib.Property(oi => oi.Name).HasMaxLength(100);
                ib.Property(oi => oi.PictureUrl);
                ib.Property(oi => oi.Quantity);
                ib.Property(oi => oi.Discount).HasPrecision(18, 2);

                ib.OwnsOne(
                    oi => oi.UnitPrice,
                    pb =>
                    {
                        pb.Property(p => p.Amount).HasPrecision(18, 2);
                    }
                );
            }
        );
        builder.Navigation(p => p.OrderItems).UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}
