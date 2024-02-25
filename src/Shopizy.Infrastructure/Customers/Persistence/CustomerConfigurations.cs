using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Shopizy.Domain.Customers;
using Shopizy.Domain.Customers.ValueObject;
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
        builder.ToTable("Customers").HasKey(c => c.Id);

        builder
            .Property(c => c.Id)
            .ValueGeneratedNever()
            .HasConversion(id => id.Value, value => CustomerId.Create(value));

        builder.Property(c => c.ProfileImageUrl);
        builder.Property(c => c.CreatedOn);
        builder.Property(c => c.ModifiedOn);
        builder.OwnsOne(c => c.Address);
        builder
            .Property(c => c.UserId)
            .HasConversion(id => id.Value, value => UserId.Create(value));
    }
}
