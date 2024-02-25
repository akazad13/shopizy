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
        builder.ToTable("Categories");
        builder.HasKey(c => c.Id);

        builder
            .Property(c => c.Id)
            .ValueGeneratedNever()
            .HasConversion(id => id.Value, value => CategoryId.Create(value));

        builder.Property(c => c.Name).HasMaxLength(100);

        builder
            .Property(c => c.ParentId)
            .HasConversion(id => id.Value, value => CategoryId.Create(value));

        builder.HasMany(c => c.Products).WithOne().OnDelete(DeleteBehavior.NoAction);
        builder.HasOne(c => c.Parent).WithMany(c => c.Children).OnDelete(DeleteBehavior.NoAction);

        builder.Navigation(p => p.Products).UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}
