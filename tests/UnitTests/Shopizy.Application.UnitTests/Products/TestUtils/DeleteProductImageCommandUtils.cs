using Shopizy.Application.Products.Commands.DeleteProductImage;

namespace Shopizy.Application.UnitTests.Products.TestUtils;

public static class DeleteProductImageCommandUtils
{
    public static DeleteProductImageCommand CreateCommand(Guid productId, Guid productImageId)
    {
        return new DeleteProductImageCommand(productId, productImageId);
    }
}
