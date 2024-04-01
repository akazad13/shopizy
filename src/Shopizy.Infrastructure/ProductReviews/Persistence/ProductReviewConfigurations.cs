using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Shopizy.Domain.ProductReviews;
using Shopizy.Domain.ProductReviews.ValueObjects;
using Shopizy.Domain.Products.ValueObjects;
using Shopizy.Domain.Users.ValueObjects;

namespace Shopizy.Infrastructure.ProductReviews.Persistence;

public class ProductReviewConfigurations : IEntityTypeConfiguration<ProductReview>
{
    public void Configure(EntityTypeBuilder<ProductReview> builder)
    {
        ConfigureProductReviewsTable(builder);
    }

    private static void ConfigureProductReviewsTable(EntityTypeBuilder<ProductReview> builder)
    {
        builder.ToTable("ProductReviews");
        builder.HasKey(pr => pr.Id);
        builder
            .Property(pr => pr.Id)
            .ValueGeneratedNever()
            .HasConversion(id => id.Value, value => ProductReviewId.Create(value));

        builder.Property(pr => pr.Comment).HasMaxLength(1000).IsRequired(false);
        builder.Property(pr => pr.CreatedOn).HasColumnType("smalldatetime");
        builder.Property(pr => pr.ModifiedOn).HasColumnType("smalldatetime");

        builder.OwnsOne(
            pr => pr.Rating,
            rb =>
            {
                rb.Property(r => r.Value).HasPrecision(18, 2);
            }
        );

        builder
            .Property(pr => pr.UserId)
            .HasConversion(id => id.Value, value => UserId.Create(value));
        builder
            .Property(pr => pr.ProductId)
            .HasConversion(id => id.Value, value => ProductId.Create(value));
    }
}
