using Shopizy.Application.UnitTests.TestUtils.Constants;
using Shopizy.Application.Users.Queries.GetUser;

namespace Shopizy.Application.UnitTests.Users.TestUtils;

public static class GetUserQueryUtils
{
    public static GetUserQuery CreateQuery() => new(Constants.User.Id.Value);
}
