using Shopizy.Application.UnitTests.TestUtils.Constants;
using Shopizy.Domain.Carts;
using Shopizy.Domain.Carts.Entities;

namespace Shopizy.Application.UnitTests.Carts.TestUtils;

public static class CartFactory
{
    public static Cart Create()
    {
        return Cart.Create(Constants.User.Id);
    }

    public static LineItem CreateLineItem()
    {
        return LineItem.Create(Constants.Product.Id);
    }
}
