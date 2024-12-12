using Shopizy.Application.UnitTests.TestUtils.Constants;
using Shopizy.Domain.Orders;

namespace Shopizy.Application.UnitTests.Orders.TestUtils;

public static class OrderFactory
{
    public static Order CreateOrder()
    {
        return Order.Create(
            Constants.User.Id,
            Constants.Order.PromoCode,
            Constants.Order.DeliveryMethod,
            Constants.Order.DeliveryCharge,
            Constants.User.Address,
            []
        );
    }
}
