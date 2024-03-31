using Shopizy.Application.Products.Commands.UpdateProduct;
using Shopizy.Application.UnitTests.TestUtils.Constants;

namespace Shopizy.Application.UnitTests.Products.TestUtils;

public static class UpdateProductCommandUtils
{
    public static UpdateProductCommand CreateCommand()
    {
        return new UpdateProductCommand(
            Constants.User.Id.Value,
            Constants.Product.Id.Value,
            Constants.Product.Name,
            Constants.Product.Description,
            Constants.Category.Id.Value,
            Constants.Product.UnitPrice,
            Constants.Product.Currency,
            Constants.Product.Discount,
            Constants.Product.Sku,
            Constants.Product.Brand,
            Constants.Product.Tags,
            Constants.Product.Barcode,
            []
        );
    }
}
