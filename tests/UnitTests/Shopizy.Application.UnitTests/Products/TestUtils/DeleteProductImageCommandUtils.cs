using Shopizy.Application.Products.Commands.DeleteProductImage;
using Shopizy.Application.UnitTests.TestUtils.Constants;

namespace Shopizy.Application.UnitTests.Products.TestUtils;

public static class DeleteProductImageCommandUtils
{
    public static DeleteProductImageCommand CreateCommand(Guid productId, Guid productImageId)
    {
        return new DeleteProductImageCommand(Constants.User.Id.Value, productId, productImageId);
    }
}
