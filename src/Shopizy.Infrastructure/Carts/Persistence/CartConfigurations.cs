using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Shopizy.Domain.Carts;
using Shopizy.Domain.Carts.Entities;
using Shopizy.Domain.Carts.ValueObjects;
using Shopizy.Domain.Products.ValueObjects;
using Shopizy.Domain.Users.ValueObjects;

namespace Shopizy.Infrastructure.Carts.Persistence;

public sealed class CartConfigurations : IEntityTypeConfiguration<Cart>
{
    public void Configure(EntityTypeBuilder<Cart> builder)
    {
        ConfigureCartsTable(builder);
        ConfigureCartProductIdsTable(builder);
    }

    private static void ConfigureCartsTable(EntityTypeBuilder<Cart> builder)
    {
        _ = builder.ToTable("Carts");
        _ = builder.HasKey(c => c.Id);

        _ = builder.HasIndex(c => c.UserId);

        _ = builder
            .Property(c => c.Id)
            .ValueGeneratedNever()
            .HasConversion(id => id.Value, value => CartId.Create(value));

        _ = builder.Property(o => o.CreatedOn).HasColumnType("smalldatetime");
        _ = builder.Property(o => o.ModifiedOn).HasColumnType("smalldatetime").IsRequired(false);

        _ = builder
            .Property(c => c.UserId)
            .HasConversion(id => id.Value, value => UserId.Create(value));
    }

    private static void ConfigureCartProductIdsTable(EntityTypeBuilder<Cart> builder)
    {
        _ = builder.OwnsMany(
            m => m.LineItems,
            ltmb =>
            {
                _ = ltmb.ToTable("LineItems");
                _ = ltmb.WithOwner().HasForeignKey("CartId");
                _ = ltmb.HasKey(nameof(LineItem.Id), "CartId");
                _ = ltmb.HasIndex("CartId", "ProductId").IsUnique();

                _ = ltmb.Property(li => li.Id)
                    .ValueGeneratedNever()
                    .HasConversion(id => id.Value, value => LineItemId.Create(value));

                _ = ltmb.Property(li => li.ProductId)
                    .ValueGeneratedNever()
                    .HasConversion(id => id.Value, value => ProductId.Create(value));
                _ = ltmb.Navigation(li => li.Product).UsePropertyAccessMode(PropertyAccessMode.Field);
            }
        );
        _ = builder.Navigation(p => p.LineItems).UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}
