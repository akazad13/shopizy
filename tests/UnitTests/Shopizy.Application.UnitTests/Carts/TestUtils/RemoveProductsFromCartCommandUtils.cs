using Shopizy.Application.Carts.Commands.RemoveProductsFromCart;
using Shopizy.Application.UnitTests.TestUtils.Constants;

namespace Shopizy.Application.UnitTests.Carts.TestUtils;

public static class RemoveProductsFromCartCommandUtils
{
    public static RemoveProductFromCartCommand CreateCommand()
    {
        return new RemoveProductFromCartCommand(
            Constants.User.Id.Value,
            Constants.Cart.Id.Value,
            [Constants.Product.Id.Value]
        );
    }
}
