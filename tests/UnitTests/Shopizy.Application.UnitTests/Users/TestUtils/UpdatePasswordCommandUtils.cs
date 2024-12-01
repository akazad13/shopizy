using Shopizy.Application.UnitTests.TestUtils.Constants;
using Shopizy.Application.Users.Commands.UpdatePassword;

namespace Shopizy.Application.UnitTests.Users.TestUtils;

public static class UpdatePasswordCommandUtils
{
    public static UpdatePasswordCommand CreateCommand()
    {
        return new UpdatePasswordCommand(
            Constants.User.Id.Value,
            Constants.User.Password,
            Constants.User.NewPassword
        );
    }

    public static UpdatePasswordCommand CreateCommandWithSameOldAndNewPassword()
    {
        return new UpdatePasswordCommand(
            Constants.User.Id.Value,
            Constants.User.Password,
            Constants.User.Password
        );
    }

    public static UpdatePasswordCommand CreateCommandWithEmptyUserId()
    {
        return new UpdatePasswordCommand(
            Constants.User.Id.Value,
            Constants.User.Password,
            Constants.User.Password
        );
    }
}
