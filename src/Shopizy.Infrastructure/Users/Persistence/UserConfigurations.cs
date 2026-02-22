using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Shopizy.Domain.Users;
using Shopizy.Domain.Users.ValueObjects;

namespace Shopizy.Infrastructure.Users.Persistence;

public sealed class UserConfigurations : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        ConfigureUsersTable(builder);
        ConfigureUserPermissionsTable(builder);
        ConfigureProductReviewIdsTable(builder);
        ConfigureOrderIdsTable(builder);
    }

    private static void ConfigureUsersTable(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("Users").HasKey(u => u.Id);
        builder.HasIndex(u => u.Email);

        builder
            .Property(u => u.Id)
            .ValueGeneratedNever()
            .HasConversion(id => id.Value, value => UserId.Create(value));

        builder.Property(u => u.FirstName).HasMaxLength(50);
        builder.Property(u => u.LastName).HasMaxLength(50);

        builder.Property(u => u.Email).HasMaxLength(50).IsRequired(true);
        builder.Property(u => u.Phone).HasMaxLength(15).IsRequired(false);

        builder.Property(u => u.Password).IsRequired(false);
        builder.Property(u => u.CreatedOn).HasColumnType("smalldatetime");
        builder.Property(u => u.ModifiedOn).HasColumnType("smalldatetime").IsRequired(false);

        builder.Property(c => c.ProfileImageUrl).IsRequired(false);
        builder.OwnsOne(
            c => c.Address,
            ab =>
            {
                ab.Property(ad => ad.Street).HasMaxLength(100).IsRequired(false);
                ab.Property(ad => ad.City).HasMaxLength(30).IsRequired(false);
                ab.Property(ad => ad.State).HasMaxLength(30).IsRequired(false);
                ab.Property(ad => ad.Country).HasMaxLength(30).IsRequired(false);
                ab.Property(ad => ad.ZipCode).HasMaxLength(10).IsRequired(false);
            }
        );

        builder.Property(u => u.CustomerId).HasMaxLength(256).IsRequired(false);

        builder.Navigation(p => p.OrderIds).UsePropertyAccessMode(PropertyAccessMode.Field);
        builder.Navigation(p => p.ProductReviewIds).UsePropertyAccessMode(PropertyAccessMode.Field);
    }

    private static void ConfigureUserPermissionsTable(EntityTypeBuilder<User> builder)
    {
        builder.OwnsMany(
            u => u.PermissionIds,
            permissionBuilder =>
            {
                permissionBuilder.ToTable("UserPermissionIds");

                permissionBuilder.WithOwner().HasForeignKey("UserId");

                permissionBuilder.HasKey("Id");

                permissionBuilder
                    .Property(d => d.Value)
                    .HasColumnName("PermissionId")
                    .ValueGeneratedNever();
            }
        );

        builder
            .Metadata.FindNavigation(nameof(User.PermissionIds))!
            .SetPropertyAccessMode(PropertyAccessMode.Field);
    }

    private static void ConfigureProductReviewIdsTable(EntityTypeBuilder<User> builder)
    {
        builder.OwnsMany(
            m => m.ProductReviewIds,
            reviewBuilder =>
            {
                reviewBuilder.ToTable("ProductReviewIds");

                reviewBuilder.WithOwner().HasForeignKey("UserId");

                reviewBuilder.HasKey("Id");

                reviewBuilder
                    .Property(r => r.Value)
                    .HasColumnName("ProductReviewId")
                    .ValueGeneratedNever();
            }
        );

        builder
            .Metadata.FindNavigation(nameof(User.ProductReviewIds))!
            .SetPropertyAccessMode(PropertyAccessMode.Field);
    }

    private static void ConfigureOrderIdsTable(EntityTypeBuilder<User> builder)
    {
        builder.OwnsMany(
            m => m.OrderIds,
            reviewBuilder =>
            {
                reviewBuilder.ToTable("OrderIds");

                reviewBuilder.WithOwner().HasForeignKey("UserId");

                reviewBuilder.HasKey("Id");

                reviewBuilder.Property(r => r.Value).HasColumnName("OrderId").ValueGeneratedNever();
            }
        );

        builder
            .Metadata.FindNavigation(nameof(User.OrderIds))!
            .SetPropertyAccessMode(PropertyAccessMode.Field);
    }
}
