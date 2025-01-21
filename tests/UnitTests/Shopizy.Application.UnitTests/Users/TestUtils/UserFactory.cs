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
            Constants.User.Email,
            Constants.User.Password,
            Constants.User.PermissionIds
        );
    }

    public static User UpdateAddress(User user)
    {
        user.UpdateAddress(
            Constants.User.Address.Street,
            Constants.User.Address.City,
            Constants.User.Address.State,
            Constants.User.Address.Country,
            Constants.User.Address.ZipCode
        );
        return user;
    }
}
