using Shopizy.Application.Carts.Queries.GetCart;
using Shopizy.Application.UnitTests.TestUtils.Constants;

namespace Shopizy.Application.UnitTests.Carts.TestUtils;

public static class GetCartQueryUtils
{
    public static GetCartQuery CreateQuery() => new(Constants.User.Id.Value);
}
