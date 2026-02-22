using Shopizy.SharedKernel.Domain.Models;
using Shopizy.Domain.Products.ValueObjects;

namespace Shopizy.Domain.Products.Entities;

public sealed class ProductImage : Entity<ProductImageId>
{
    public string ImageUrl { get; set; } = null!;
    public int Seq { get; set; }
    public string PublicId { get; set; } = null!;

    public static ProductImage Create(string productUrl, int seq, string publicId)
    {
        return new ProductImage(ProductImageId.CreateUnique(), productUrl, seq, publicId);
    }

    private ProductImage(ProductImageId productImageId, string imageUrl, int seq, string publicId)
        : base(productImageId)
    {
        ImageUrl = imageUrl;
        Seq = seq;
        PublicId = publicId;
    }

    private ProductImage() { }
}
