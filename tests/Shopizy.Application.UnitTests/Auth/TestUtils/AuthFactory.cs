using Shopizy.Application.UnitTests.TestUtils.Constants;
using Shopizy.Domain.Permissions;

namespace Shopizy.Application.UnitTests.Auth.TestUtils;

public static class AuthFactory
{
    public static IReadOnlyList<Permission> GetPermissions()
    {
        return
        [
            Constants.PermissionConstants.GetCategoryPermission,
            Constants.PermissionConstants.GetProductPermission,
            Constants.PermissionConstants.GetCartPermission,
            Constants.PermissionConstants.GetOrderPermission,
        ];
    }
}
