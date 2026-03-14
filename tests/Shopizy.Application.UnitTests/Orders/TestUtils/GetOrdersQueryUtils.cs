using Shopizy.Application.Orders.Queries.GetOrders;
using Shopizy.Application.UnitTests.TestUtils.Constants;

namespace Shopizy.Application.UnitTests.Orders.TestUtils;

public static class GetOrdersQueryUtils
{
    public static GetOrdersQuery CreateQuery()
    {
        return new GetOrdersQuery(
            Constants.User.Id.Value,
            DateTime.Now.AddDays(-90),
            DateTime.Now,
            null,
            1,
            10
        );
    }
}
