using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Shopizy.Domain.Users;
using Shopizy.Domain.Users.ValueObject;

namespace Shopizy.Infrastructure.Users.Persistence;

public sealed class UserConfigurations : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        ConfigureUsersTable(builder);
    }

    private static void ConfigureUsersTable(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("Users");

        builder.HasKey(u => u.Id);
        builder.HasIndex(u => u.Phone);

        builder
            .Property(u => u.Id)
            .ValueGeneratedNever()
            .HasConversion(id => id.Value, value => UserId.Create(value));

        builder.Property(u => u.FirstName).HasMaxLength(50);

        builder.Property(u => u.LastName).HasMaxLength(50);

        builder.Property(u => u.Phone).HasMaxLength(15);

        builder.Property(u => u.Password);
    }
}
