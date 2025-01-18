using Shopizy.Application.Orders.Queries.GetOrders;

namespace Shopizy.Application.UnitTests.Orders.TestUtils;

public static class GetOrdersQueryUtils
{
    public static GetOrdersQuery CreateQuery()
    {
        return new GetOrdersQuery();
    }
}
