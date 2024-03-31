using Shopizy.Application.Carts.Queries.GetCart;
using Shopizy.Application.UnitTests.TestUtils.Constants;

namespace Shopizy.Application.UnitTests.Carts.TestUtils;

public static class GetCartQueryUtils
{
    public static GetCartQuery CreateQuery()
    {
        return new GetCartQuery(Constants.User.Id.Value, Constants.Customer.Id.Value);
    }
}
