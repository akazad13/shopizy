using Shopizy.Domain.Common.Models;
using Shopizy.Domain.Products.ValueObjects;

namespace Shopizy.Domain.Products.Entities;

public sealed class ProductImage : Entity<ProductImageId>
{
    public ProductId ProductId { get; set; }
    public string ProductUrl { get; set; }

    public static ProductImage Create(ProductId productId, string productUrl)
    {
        return new ProductImage(ProductImageId.CreateUnique(), productId, productUrl);
    }

    private ProductImage(ProductImageId id, ProductId productId, string productUrl) : base(id)
    {
        ProductId = productId;
        ProductUrl = productUrl;
    }

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    private ProductImage() { }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
}
