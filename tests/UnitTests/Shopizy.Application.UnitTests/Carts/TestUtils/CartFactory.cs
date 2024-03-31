using Shopizy.Application.UnitTests.TestUtils.Constants;
using Shopizy.Domain.Carts;
using Shopizy.Domain.Carts.Entities;

namespace Shopizy.Application.UnitTests.Carts.TestUtils;

public class CartFactory
{
    public static Cart Create()
    {
        return Cart.Create(Constants.Customer.Id);
    }

    public static LineItem CreateLineItem()
    {
        return LineItem.Create(Constants.Product.Id);
    }
}
