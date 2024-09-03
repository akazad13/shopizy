using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Shopizy.Domain.Orders;
using Shopizy.Domain.Orders.Entities;
using Shopizy.Domain.Orders.ValueObjects;
using Shopizy.Domain.Users.ValueObjects;

namespace Shopizy.Infrastructure.Orders.Persistence;

public sealed class OrderConfigurations : IEntityTypeConfiguration<Order>
{
    public void Configure(EntityTypeBuilder<Order> builder)
    {
        ConfigureOrdersTable(builder);
        ConfigureOrderItemsTable(builder);
    }

    private static void ConfigureOrdersTable(EntityTypeBuilder<Order> builder)
    {
        _ = builder.ToTable("Orders");
        _ = builder.HasKey(c => c.Id);

        _ = builder
            .Property(c => c.Id)
            .ValueGeneratedNever()
            .HasConversion(id => id.Value, value => OrderId.Create(value));

        _ = builder.Property(o => o.PromoCode).HasMaxLength(15).IsRequired(false);
        _ = builder.Property(o => o.CreatedOn).HasColumnType("smalldatetime");
        _ = builder.Property(o => o.ModifiedOn).HasColumnType("smalldatetime").IsRequired(false);
        _ = builder.Property(o => o.OrderStatus);
        _ = builder.Property(o => o.CancellationReason).IsRequired(false).HasMaxLength(200);

        _ = builder.OwnsOne(
            o => o.DeliveryCharge,
            pb =>
            {
                _ = pb.Property(p => p.Amount).HasPrecision(18, 2);
            }
        );
        _ = builder.OwnsOne(
            o => o.ShippingAddress,
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
            .Property(c => c.UserId)
            .HasConversion(id => id.Value, value => UserId.Create(value));
    }

    private static void ConfigureOrderItemsTable(EntityTypeBuilder<Order> builder)
    {
        _ = builder.OwnsMany(
            o => o.OrderItems,
            ib =>
            {
                _ = ib.ToTable("OrderItems");
                _ = ib.WithOwner().HasForeignKey("OrderId");
                _ = ib.HasKey(nameof(OrderItem.Id), "OrderId");

                _ = ib.Property(oi => oi.Id)
                    .ValueGeneratedNever()
                    .HasConversion(id => id.Value, value => OrderItemId.Create(value));

                _ = ib.Property(oi => oi.Name).HasMaxLength(100);
                _ = ib.Property(oi => oi.PictureUrl).IsRequired(false);
                _ = ib.Property(oi => oi.Quantity);
                _ = ib.Property(oi => oi.Discount).HasPrecision(18, 2);

                _ = ib.OwnsOne(
                    oi => oi.UnitPrice,
                    pb =>
                    {
                        _ = pb.Property(p => p.Amount).HasPrecision(18, 2);
                    }
                );
            }
        );
        _ = builder.Navigation(p => p.OrderItems).UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}
