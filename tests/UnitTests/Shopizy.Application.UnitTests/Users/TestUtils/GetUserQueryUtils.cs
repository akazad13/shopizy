using Shopizy.Application.Users.Queries.GetUser;
using Shopizy.Application.UnitTests.TestUtils.Constants;

namespace Shopizy.Application.UnitTests.Users.TestUtils;

public static class GetUserQueryUtils
{
    public static GetUserQuery CreateQuery()
    {
        return new GetUserQuery(Constants.User.Id.Value);
    }
}
