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
        ConfigureShipmentTable(builder);
    }

    private static void ConfigureOrdersTable(EntityTypeBuilder<Order> builder)
    {
        builder.ToTable("Orders");
        builder.HasKey(c => c.Id);

        builder
            .Property(c => c.Id)
            .ValueGeneratedNever()
            .HasConversion(id => id.Value, value => OrderId.Create(value));

        builder.Property(o => o.PromoCode).HasMaxLength(15).IsRequired(false);
        builder.Property(o => o.CreatedOn).HasColumnType("smalldatetime");
        builder.Property(o => o.ModifiedOn).HasColumnType("smalldatetime").IsRequired(false);
        builder.Property(o => o.OrderStatus);
        builder.Property(o => o.CancellationReason).IsRequired(false).HasMaxLength(200);

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
                ab.Property(ad => ad.Street).HasMaxLength(100);
                ab.Property(ad => ad.City).HasMaxLength(30);
                ab.Property(ad => ad.State).HasMaxLength(30);
                ab.Property(ad => ad.Country).HasMaxLength(30);
                ab.Property(ad => ad.ZipCode).HasMaxLength(10);
            }
        );
        builder
            .Property(c => c.UserId)
            .HasConversion(id => id.Value, value => UserId.Create(value));
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
                ib.Property(oi => oi.PictureUrl).IsRequired(false);
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

    private static void ConfigureShipmentTable(EntityTypeBuilder<Order> builder)
    {
        builder.OwnsOne(
            o => o.Shipment,
            sb =>
            {
                sb.ToTable("Shipments");
                sb.WithOwner().HasForeignKey("OrderId");
                sb.HasKey(nameof(Shipment.Id), "OrderId");
                sb.Property(s => s.Id)
                    .ValueGeneratedNever()
                    .HasConversion(id => id.Value, v => ShipmentId.Create(v));
                sb.Property(s => s.Carrier).HasMaxLength(100);
                sb.Property(s => s.TrackingNumber).HasMaxLength(100);
                sb.Property(s => s.EstimatedDelivery).IsRequired(false);
                sb.Property(s => s.Status).HasConversion<string>();
                sb.Property(s => s.CreatedOn).HasColumnType("smalldatetime");
                sb.Property(s => s.ModifiedOn).HasColumnType("smalldatetime").IsRequired(false);
            }
        );
    }
}
