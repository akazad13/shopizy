using Shopizy.Application.Products.Commands.CreateProduct;
using Shopizy.Application.UnitTests.TestUtils.Constants;
using Shopizy.Domain.Categories.ValueObjects;

namespace Shopizy.Application.UnitTests.Products.TestUtils;

public static class CreateProductCommandUtils
{
    public static CreateProductCommand CreateCommand()
    {
        return new CreateProductCommand(
            Constants.User.Id.Value,
            Constants.Product.Name,
            Constants.Product.ShortDescription,
            Constants.Product.Description,
            CategoryId.Create(Constants.Category.Id.Value),
            Constants.Product.Price,
            Constants.Product.Discount,
            Constants.Product.Sku,
            Constants.Product.StockQuantity,
            Constants.Product.BrandId,
            Constants.Product.Colors,
            Constants.Product.Sizes,
            Constants.Product.Tags,
            Constants.Product.Barcode,
            []
        );
    }

    public static CreateProductCommand CreateCommandWithEmptyProductName()
    {
        return new CreateProductCommand(
            Constants.User.Id.Value,
            "",
            Constants.Product.ShortDescription,
            Constants.Product.Description,
            CategoryId.Create(Constants.Category.Id.Value),
            Constants.Product.Price,
            Constants.Product.Discount,
            Constants.Product.Sku,
            Constants.Product.StockQuantity,
            Constants.Product.BrandId,
            Constants.Product.Colors,
            Constants.Product.Sizes,
            Constants.Product.Tags,
            Constants.Product.Barcode,
            []
        );
    }
}
