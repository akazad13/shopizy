using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Shopizy.Domain.Categories;
using Shopizy.Domain.Categories.ValueObjects;

namespace Shopizy.Infrastructure.Categories.Persistence;

public sealed class CategoryConfigurations : IEntityTypeConfiguration<Category>
{
    public void Configure(EntityTypeBuilder<Category> builder)
    {
        ConfigureCategoriesTable(builder);
    }

    private static void ConfigureCategoriesTable(EntityTypeBuilder<Category> builder)
    {
        _ = builder.ToTable("Categories");
        _ = builder.HasKey(c => c.Id);

        _ = builder
            .Property(c => c.Id)
            .ValueGeneratedNever()
            .HasConversion(id => id.Value, value => CategoryId.Create(value));

        _ = builder.Property(c => c.Name).HasMaxLength(100);
        _ = builder.Property(c => c.ParentId).IsRequired(false);

        _ = builder.HasMany(c => c.Products).WithOne().OnDelete(DeleteBehavior.NoAction);
        _ = builder.Navigation(p => p.Products).UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}
