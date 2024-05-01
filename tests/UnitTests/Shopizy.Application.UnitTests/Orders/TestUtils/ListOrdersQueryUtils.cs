using Shopizy.Application.Orders.Queries.ListOrders;
using Shopizy.Application.UnitTests.TestUtils.Constants;

namespace Shopizy.Application.UnitTests.Orders.TestUtils;

public static class ListOrdersQueryUtils
{
    public static ListOrdersQuery CreateQuery()
    {
        return new ListOrdersQuery(Constants.User.Id.Value);
    }
}
