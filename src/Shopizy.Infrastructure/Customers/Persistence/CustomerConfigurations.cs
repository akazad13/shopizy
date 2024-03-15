using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Shopizy.Domain.Customers;
using Shopizy.Domain.Customers.ValueObjects;
using Shopizy.Domain.Users.ValueObjects;

namespace shopizy.Infrastructure.Customers.Persistence;

public sealed class CustomerConfigurations : IEntityTypeConfiguration<Customer>
{
    public void Configure(EntityTypeBuilder<Customer> builder)
    {
        ConfigureCustomersTable(builder);
    }

    private static void ConfigureCustomersTable(EntityTypeBuilder<Customer> builder)
    {
        builder.ToTable("Customers");
        builder.HasKey(c => c.Id);

        builder.HasOne(c => c.User).WithOne().OnDelete(DeleteBehavior.NoAction);

        builder
            .Property(c => c.Id)
            .ValueGeneratedNever()
            .HasConversion(id => id.Value, value => CustomerId.Create(value));

        builder.Property(c => c.ProfileImageUrl).IsRequired(false);
        builder.Property(c => c.CreatedOn).HasColumnType("smalldatetime");
        builder.Property(c => c.ModifiedOn).HasColumnType("smalldatetime");
        builder.OwnsOne(
            c => c.Address,
            ab =>
            {
                ab.Property(ad => ad.Line).HasMaxLength(100).IsRequired(false);
                ab.Property(ad => ad.City).HasMaxLength(30).IsRequired(false);
                ab.Property(ad => ad.State).HasMaxLength(30).IsRequired(false);
                ab.Property(ad => ad.Country).HasMaxLength(30).IsRequired(false);
                ab.Property(ad => ad.ZipCode).HasMaxLength(10).IsRequired(false);
            }
        );
        builder
            .Property(c => c.UserId)
            .HasConversion(id => id.Value, value => UserId.Create(value));

        builder.Navigation(p => p.Orders).UsePropertyAccessMode(PropertyAccessMode.Field);
        builder.Navigation(p => p.ProductReviews).UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}
