using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Shopizy.Domain.Permissions;
using Shopizy.Domain.Permissions.ValueObjects;

namespace Shopizy.Infrastructure.Permissions.Persistence;

public sealed class PermissionConfigurations : IEntityTypeConfiguration<Permission>
{
    public void Configure(EntityTypeBuilder<Permission> builder)
    {
        ConfigureUsersTable(builder);
    }

    private static void ConfigureUsersTable(EntityTypeBuilder<Permission> builder)
    {
        builder.ToTable("Permissions").HasKey(p => p.Id);

        builder
            .Property(p => p.Id)
            .ValueGeneratedNever()
            .HasConversion(id => id.Value, value => PermissionId.Create(value));

        builder.Property(u => u.Name).HasMaxLength(50);
    }
}
