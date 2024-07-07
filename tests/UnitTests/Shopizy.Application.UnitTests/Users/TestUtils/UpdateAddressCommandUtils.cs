using Shopizy.Application.UnitTests.TestUtils.Constants;
using Shopizy.Application.Users.Commands.UpdateAddress;

namespace Shopizy.Application.UnitTests.Users.TestUtils;

public static class UpdateAddressCommandUtils
{
    public static UpdateAddressCommand CreateCommand()
    {
        return new UpdateAddressCommand(
            Constants.User.Id.Value,
            Constants.User.Address.Line,
            Constants.User.Address.City,
            Constants.User.Address.State,
            Constants.User.Address.Country,
            Constants.User.Address.ZipCode
        );
    }
}
