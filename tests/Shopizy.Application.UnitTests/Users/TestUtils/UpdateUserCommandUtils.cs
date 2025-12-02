using Shopizy.Application.UnitTests.TestUtils.Constants;
using Shopizy.Application.Users.Commands.UpdateUser;

namespace Shopizy.Application.UnitTests.Users.TestUtils;

public static class UpdateUserCommandUtils
{
    public static UpdateUserCommand CreateCommand()
    {
        return new UpdateUserCommand(
            Constants.User.Id.Value,
            Constants.User.FirstName,
            Constants.User.LastName,
            Constants.User.Email,
            "1234567890",
            Constants.User.Address.Street,
            Constants.User.Address.City,
            Constants.User.Address.State,
            Constants.User.Address.Country,
            Constants.User.Address.ZipCode
        );
    }
}
