using Shopizy.Domain.Permissions;

namespace Shopizy.Application.UnitTests.TestUtils.Constants;

public static partial class Constants
{
    public static class PermissionConstants
    {
        public static readonly Permission GetCategoryPermission = Permission.Create("get:category");
        public static readonly Permission GetProductPermission = Permission.Create("get:product");
        public static readonly Permission GetCartPermission = Permission.Create("get:cart");
        public static readonly Permission GetOrderPermission = Permission.Create("get:order");
    }
}
