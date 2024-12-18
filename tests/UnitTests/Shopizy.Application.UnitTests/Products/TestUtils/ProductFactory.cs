using Shopizy.Application.UnitTests.TestUtils.Constants;
using Shopizy.Domain.Products;
using Shopizy.Domain.Products.Entities;

namespace Shopizy.Application.UnitTests.Products.TestUtils;

public static class ProductFactory
{
    public static Product CreateProduct()
    {
        return Product.Create(
            Constants.Product.Name,
            Constants.Product.ShortDescription,
            Constants.Product.Description,
            Constants.Category.Id,
            Constants.Product.Sku,
            Constants.Product.Price,
            Constants.Product.Discount,
            Constants.Product.Brand,
            Constants.Product.Tags,
            Constants.Product.Colors,
            Constants.Product.Sizes,
            Constants.Product.Barcode
        );
    }

    public static ProductImage CreateProductImage()
    {
        return ProductImage.Create(
            Constants.ProductImage.ImageUrl,
            Constants.ProductImage.Seq,
            Constants.ProductImage.PublicId
        );
    }
}
