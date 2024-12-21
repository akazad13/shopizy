using Shopizy.Application.Authentication.Queries.login;
using Shopizy.Application.UnitTests.TestUtils.Constants;

namespace Shopizy.Application.UnitTests.Authentication.TestUtils;

public static class LoginQueryUtils
{
    public static LoginQuery CreateQuery()
    {
        return new LoginQuery(Constants.User.Phone, Constants.User.Password);
    }
}
