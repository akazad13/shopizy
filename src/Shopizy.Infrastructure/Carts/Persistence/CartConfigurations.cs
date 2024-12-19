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
        ConfigureCartItemsTable(builder);
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
        builder.Property(o => o.ModifiedOn).HasColumnType("smalldatetime").IsRequired(false);

        builder
            .Property(c => c.UserId)
            .HasConversion(id => id.Value, value => UserId.Create(value));
    }

    private static void ConfigureCartItemsTable(EntityTypeBuilder<Cart> builder)
    {
        builder.OwnsMany(
            m => m.CartItems,
            ci =>
            {
                ci.ToTable("CartItems");
                ci.WithOwner().HasForeignKey("CartId");
                ci.HasKey(nameof(CartItem.Id), "CartId");

                ci.Property(li => li.Id)
                    .ValueGeneratedNever()
                    .HasConversion(id => id.Value, value => CartItemId.Create(value));

                ci.Property(li => li.ProductId)
                    .ValueGeneratedNever()
                    .HasConversion(id => id.Value, value => ProductId.Create(value));
                ci.Navigation(li => li.Product).UsePropertyAccessMode(PropertyAccessMode.Field);
            }
        );
        builder.Navigation(p => p.CartItems).UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}
