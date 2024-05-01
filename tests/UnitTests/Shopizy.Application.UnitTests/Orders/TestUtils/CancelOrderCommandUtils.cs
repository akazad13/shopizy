using Shopizy.Application.Orders.Commands.CancelOrder;
using Shopizy.Application.UnitTests.TestUtils.Constants;
using Shopizy.Domain.Orders.ValueObjects;

namespace Shopizy.Application.UnitTests.Orders.TestUtils;

public static class CancelOrderCommandUtils
{
    public static CancelOrderCommand CreateCommand(OrderId orderId)
    {
        return new CancelOrderCommand(
            Constants.User.Id.Value,
            orderId.Value,
            Constants.Order.CancellationReason
        );
    }
}
