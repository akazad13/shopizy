using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Shopizy.Domain.Customers.ValueObject;
using Shopizy.Domain.ProductReviews;
using Shopizy.Domain.ProductReviews.ValueObjects;
using Shopizy.Domain.Products.ValueObjects;

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

        builder.Property(pr => pr.Comment).HasMaxLength(1000);
        builder.Property(pr => pr.CreatedOn);
        builder.Property(pr => pr.ModifiedOn);

        builder.OwnsOne(pr => pr.Rating);

        builder
            .Property(pr => pr.CustomerId)
            .HasConversion(id => id.Value, value => CustomerId.Create(value));
        builder
            .Property(pr => pr.ProductId)
            .HasConversion(id => id.Value, value => ProductId.Create(value));
    }
}
