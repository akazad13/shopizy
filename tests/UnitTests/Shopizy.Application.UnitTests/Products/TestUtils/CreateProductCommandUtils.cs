using Shopizy.Application.Products.Commands.CreateProduct;
using Shopizy.Application.UnitTests.TestUtils.Constants;

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
            Constants.Category.Id.Value,
            Constants.Product.UnitPrice,
            Constants.Product.Currency,
            Constants.Product.Discount,
            Constants.Product.Sku,
            Constants.Product.Brand,
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
            Constants.Category.Id.Value,
            Constants.Product.UnitPrice,
            Constants.Product.Currency,
            Constants.Product.Discount,
            Constants.Product.Sku,
            Constants.Product.Brand,
            Constants.Product.Tags,
            Constants.Product.Colors,
            Constants.Product.Sizes,
            Constants.Product.Barcode,
            []
        );
    }
}
