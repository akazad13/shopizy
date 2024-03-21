using Shopizy.Application.UnitTests.TestUtils.Constants;
using Shopizy.Application.Products.Commands.CreateProduct;

namespace Shopizy.Application.UnitTests.Products.Commands.TestUtils;

public static class CreateProductCommandUtils
{
    public static CreateProductCommand CreateCommand()
    {
        return new CreateProductCommand(
            Constants.User.Id,
            Constants.Product.Name,
            Constants.Product.Description,
            Constants.Category.Id,
            Constants.Product.UnitPrice,
            Constants.Product.Currency,
            Constants.Product.Discount,
            Constants.Product.Sku,
            Constants.Product.Brand,
            Constants.Product.Tags,
            Constants.Product.Barcode,
            Constants.Product.StockQuantity,
            null
        );
    }
}
