using Shopizy.Application.Products.Commands.DeleteProduct;
using Shopizy.Application.UnitTests.TestUtils.Constants;

namespace Shopizy.Application.UnitTests.Products.TestUtils;

public static class DeleteProductCommandUtils
{
    public static DeleteProductCommand CreateCommand()
    {
        return new DeleteProductCommand(Constants.Product.Id.Value);
    }
}
