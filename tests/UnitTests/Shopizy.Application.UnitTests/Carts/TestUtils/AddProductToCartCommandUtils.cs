using Shopizy.Application.Carts.Commands.AddProductToCart;
using Shopizy.Application.UnitTests.TestUtils.Constants;

namespace Shopizy.Application.UnitTests.Carts.TestUtils;

public static class AddProductToCartCommandUtils
{
    public static AddProductToCartCommand CreateCommand()
    {
        return new AddProductToCartCommand(
            Constants.Cart.Id.Value,
            Constants.Product.Id.Value,
            Constants.CartItem.Color,
            Constants.CartItem.Size,
            1
        );
    }
}
