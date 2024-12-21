using Shopizy.Application.Carts.Commands.UpdateProductQuantity;
using Shopizy.Application.UnitTests.TestUtils.Constants;

namespace Shopizy.Application.UnitTests.Carts.TestUtils;

public static class UpdateProductQuantityCommandUtils
{
    public static UpdateProductQuantityCommand CreateCommand(int quantity)
    {
        return new UpdateProductQuantityCommand(
            Constants.Cart.Id.Value,
            Constants.CartItem.Id.Value,
            Constants.Product.Id.Value,
            quantity
        );
    }
}
