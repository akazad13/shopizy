using Shopizy.Application.Carts.Commands.CreateCartWithFirstProduct;
using Shopizy.Application.UnitTests.TestUtils.Constants;

namespace Shopizy.Application.UnitTests.Carts.TestUtils;

public static class CreateCartWithFirstProductCommandUtils
{
    public static CreateCartWithFirstProductCommand CreateCommand()
    {
        return new CreateCartWithFirstProductCommand(
            Constants.User.Id.Value,
            Constants.Customer.Id.Value,
            Constants.Product.Id.Value
        );
    }
}
