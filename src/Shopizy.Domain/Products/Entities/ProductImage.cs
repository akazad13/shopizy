using Shopizy.Domain.Common.Models;
using Shopizy.Domain.Products.ValueObjects;

namespace Shopizy.Domain.Products.Entities;

public sealed class ProductImage : Entity<ProductImageId>
{
    public string ImageUrl { get; set; }
    public int Seq { get; set; }

    public static ProductImage Create(string productUrl, int seq)
    {
        return new ProductImage(ProductImageId.CreateUnique(), productUrl, seq);
    }

    private ProductImage(ProductImageId productImageId, string imageUrl, int seq)
        : base(productImageId)
    {
        ImageUrl = imageUrl;
        Seq = seq;
    }

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    private ProductImage() { }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
}
