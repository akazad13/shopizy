using Shopizy.Application.Orders.Commands.CancelOrder;
using Shopizy.Application.UnitTests.TestUtils.Constants;

namespace Shopizy.Application.UnitTests.Orders.TestUtils;

public static class CancelOrderCommandUtils
{
    public static CancelOrderCommand CreateCommand(Guid? orderId = null)
    {
        return new CancelOrderCommand(
            Constants.User.Id.Value,
            orderId ?? Constants.Order.Id.Value,
            Constants.Order.CancellationReason
        );
    }
}
