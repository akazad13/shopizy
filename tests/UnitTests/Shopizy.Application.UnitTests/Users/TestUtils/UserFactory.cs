using Shopizy.Application.UnitTests.TestUtils.Constants;
using Shopizy.Domain.Users;

namespace Shopizy.Application.UnitTests.Users.TestUtils;

public static class UserFactory
{
    public static User CreateUser()
    {
        return User.Create(
            Constants.User.FirstName,
            Constants.User.LastName,
            Constants.User.Phone,
            Constants.User.Password
        );
    }

    public static User UpdateAddress(User user)
    {
        user.UpdateAddress(Constants.User.Address);
        return user;
    }
}
