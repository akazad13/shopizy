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
        builder.ToTable("Users").HasKey(u => u.Id);
        builder.HasIndex(u => u.Phone);

        builder
            .Property(u => u.Id)
            .ValueGeneratedNever()
            .HasConversion(id => id.Value, value => UserId.Create(value));

        builder.Property(u => u.FirstName).HasMaxLength(50);

        builder.Property(u => u.LastName).HasMaxLength(50);

        builder.Property(u => u.Phone).HasMaxLength(15);

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

        builder.Navigation(p => p.Orders).UsePropertyAccessMode(PropertyAccessMode.Field);
        builder.Navigation(p => p.ProductReviews).UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}
