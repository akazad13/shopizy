using Shopizy.Application.Orders.Queries.GetOrder;
using Shopizy.Application.UnitTests.TestUtils.Constants;

namespace Shopizy.Application.UnitTests.Orders.TestUtils;

public static class GetOrderQueryUtils
{
    public static GetOrderQuery CreateQuery()
    {
        return new GetOrderQuery(Constants.User.Id.Value, Constants.Order.Id.Value);
    }
}
