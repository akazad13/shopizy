using Shopizy.Application.Auth.Queries.login;
using Shopizy.Application.UnitTests.TestUtils.Constants;

namespace Shopizy.Application.UnitTests.Auth.TestUtils;

public static class LoginQueryUtils
{
    public static LoginQuery CreateQuery()
    {
        return new LoginQuery(Constants.User.Email, Constants.User.Password);
    }
}
