using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Shopizy.Domain.Products.ValueObjects;
using Shopizy.Domain.Users.ValueObjects;
using Shopizy.Domain.Wishlists;
using Shopizy.Domain.Wishlists.Entities;
using Shopizy.Domain.Wishlists.ValueObjects;

namespace Shopizy.Infrastructure.Wishlists.Persistence;

public sealed class WishlistConfigurations : IEntityTypeConfiguration<Wishlist>
{
    public void Configure(EntityTypeBuilder<Wishlist> builder)
    {
        ConfigureWishlistsTable(builder);
        ConfigureWishlistItemsTable(builder);
    }

    private static void ConfigureWishlistsTable(EntityTypeBuilder<Wishlist> builder)
    {
        builder.ToTable("Wishlists");
        builder.HasKey(w => w.Id);
        builder.HasIndex(w => w.UserId).IsUnique();

        builder
            .Property(w => w.Id)
            .ValueGeneratedNever()
            .HasConversion(id => id.Value, value => WishlistId.Create(value));

        builder
            .Property(w => w.UserId)
            .HasConversion(id => id.Value, value => UserId.Create(value));

        builder.Property(w => w.CreatedOn).HasColumnType("smalldatetime");
        builder.Property(w => w.ModifiedOn).HasColumnType("smalldatetime").IsRequired(false);
        builder.Property(w => w.Name).HasMaxLength(100).IsRequired(false);
        builder.Property(w => w.IsPublic).HasDefaultValue(false);
    }

    private static void ConfigureWishlistItemsTable(EntityTypeBuilder<Wishlist> builder)
    {
        builder.OwnsMany(
            w => w.WishlistItems,
            wi =>
            {
                wi.ToTable("WishlistItems");
                wi.WithOwner().HasForeignKey("WishlistId");
                wi.HasKey(nameof(WishlistItem.Id), "WishlistId");

                wi.Property(i => i.Id)
                    .ValueGeneratedNever()
                    .HasConversion(id => id.Value, value => WishlistItemId.Create(value));

                wi.Property(i => i.ProductId)
                    .ValueGeneratedNever()
                    .HasConversion(id => id.Value, value => ProductId.Create(value));
            }
        );
        builder.Navigation(w => w.WishlistItems).UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}
