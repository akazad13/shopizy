using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Shopizy.Domain.Brands;
using Shopizy.Domain.Brands.ValueObjects;

namespace Shopizy.Infrastructure.Brands.Persistence;

public sealed class BrandConfigurations : IEntityTypeConfiguration<Brand>
{
    public void Configure(EntityTypeBuilder<Brand> builder)
    {
        builder.ToTable("Brands").HasKey(brand => brand.Id);

        builder
            .Property(brand => brand.Id)
            .ValueGeneratedNever()
            .HasConversion(id => id.Value, value => BrandId.Create(value));

        builder.Property(brand => brand.Name).HasMaxLength(50).IsRequired();
        builder.Property(brand => brand.LogoUrl).HasMaxLength(500).IsRequired(false);
        builder.Property(brand => brand.Country).HasMaxLength(100).IsRequired();

        builder.HasIndex(brand => brand.Name).IsUnique();
    }
}