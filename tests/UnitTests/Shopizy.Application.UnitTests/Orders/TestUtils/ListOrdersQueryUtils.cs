using Shopizy.Application.Orders.Queries.ListOrders;

namespace Shopizy.Application.UnitTests.Orders.TestUtils;

public static class ListOrdersQueryUtils
{
    public static ListOrdersQuery CreateQuery()
    {
        return new ListOrdersQuery();
    }
}
