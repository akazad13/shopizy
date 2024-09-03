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
    }

    private static void ConfigureUsersTable(EntityTypeBuilder<User> builder)
    {
        _ = builder.ToTable("Users").HasKey(u => u.Id);
        _ = builder.HasIndex(u => u.Phone);

        _ = builder
            .Property(u => u.Id)
            .ValueGeneratedNever()
            .HasConversion(id => id.Value, value => UserId.Create(value));

        _ = builder.Property(u => u.FirstName).HasMaxLength(50);

        _ = builder.Property(u => u.LastName).HasMaxLength(50);

        _ = builder.Property(u => u.Phone).HasMaxLength(15);

        _ = builder.Property(u => u.Password).IsRequired(false);
        _ = builder.Property(u => u.CreatedOn).HasColumnType("smalldatetime");
        _ = builder.Property(u => u.ModifiedOn).HasColumnType("smalldatetime").IsRequired(false);

        _ = builder.Property(c => c.ProfileImageUrl).IsRequired(false);
        _ = builder.OwnsOne(
            c => c.Address,
            ab =>
            {
                _ = ab.Property(ad => ad.Street).HasMaxLength(100).IsRequired(false);
                _ = ab.Property(ad => ad.City).HasMaxLength(30).IsRequired(false);
                _ = ab.Property(ad => ad.State).HasMaxLength(30).IsRequired(false);
                _ = ab.Property(ad => ad.Country).HasMaxLength(30).IsRequired(false);
                _ = ab.Property(ad => ad.ZipCode).HasMaxLength(10).IsRequired(false);
            }
        );

        _ = builder.Navigation(p => p.Orders).UsePropertyAccessMode(PropertyAccessMode.Field);
        _ = builder.Navigation(p => p.ProductReviews).UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}
