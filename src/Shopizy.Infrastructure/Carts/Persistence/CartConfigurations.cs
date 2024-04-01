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
        builder.ToTable("Carts");
        builder.HasKey(c => c.Id);

        builder.HasIndex(c => c.UserId);

        builder
            .Property(c => c.Id)
            .ValueGeneratedNever()
            .HasConversion(id => id.Value, value => CartId.Create(value));

        builder.Property(o => o.CreatedOn).HasColumnType("smalldatetime");
        builder.Property(o => o.ModifiedOn).HasColumnType("smalldatetime");

        builder
            .Property(c => c.UserId)
            .HasConversion(id => id.Value, value => UserId.Create(value));
    }

    private static void ConfigureCartProductIdsTable(EntityTypeBuilder<Cart> builder)
    {
        builder.OwnsMany(
            m => m.LineItems,
            ltmb =>
            {
                ltmb.ToTable("LineItems");
                ltmb.WithOwner().HasForeignKey("CartId");
                ltmb.HasKey(nameof(LineItem.Id), "CartId");
                ltmb.HasIndex("CartId", "ProductId").IsUnique();

                ltmb.Property(li => li.Id)
                    .ValueGeneratedNever()
                    .HasConversion(id => id.Value, value => LineItemId.Create(value));

                ltmb.Property(li => li.ProductId)
                    .ValueGeneratedNever()
                    .HasConversion(id => id.Value, value => ProductId.Create(value));
                ltmb.Navigation(li => li.Product).UsePropertyAccessMode(PropertyAccessMode.Field);
            }
        );
        builder.Navigation(p => p.LineItems).UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}
