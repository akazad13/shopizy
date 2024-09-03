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
        _ = builder.ToTable("ProductReviews");
        _ = builder.HasKey(pr => pr.Id);
        _ = builder
            .Property(pr => pr.Id)
            .ValueGeneratedNever()
            .HasConversion(id => id.Value, value => ProductReviewId.Create(value));

        _ = builder.Property(pr => pr.Comment).HasMaxLength(1000).IsRequired(false);
        _ = builder.Property(pr => pr.CreatedOn).HasColumnType("smalldatetime");
        _ = builder.Property(pr => pr.ModifiedOn).HasColumnType("smalldatetime").IsRequired(false);

        _ = builder.OwnsOne(
            pr => pr.Rating,
            rb =>
            {
                _ = rb.Property(r => r.Value).HasPrecision(18, 2);
            }
        );

        _ = builder
            .Property(pr => pr.UserId)
            .HasConversion(id => id.Value, value => UserId.Create(value));
        _ = builder
            .Property(pr => pr.ProductId)
            .HasConversion(id => id.Value, value => ProductId.Create(value));
    }
}
