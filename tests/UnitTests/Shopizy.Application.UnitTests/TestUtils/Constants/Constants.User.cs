using Shopizy.Domain.Orders.ValueObjects;
using Shopizy.Domain.Permissions.ValueObjects;
using Shopizy.Domain.Users.ValueObjects;

namespace Shopizy.Application.UnitTests.TestUtils.Constants;

public static partial class Constants
{
    public static class User
    {
        public static readonly UserId Id = UserId.Create(
            new Guid("88d9af82-bef4-4042-8e55-b01b89f23e68")
        );

        public const string FirstName = "John";
        public const string LastName = "Doe";
        public const string ProfileImageUrl = "";
        public const string Email = "test@test.com";
        public const string Password = "oldPassword";
        public const string NewPassword = "newPassword";
        public static readonly Address Address = Address.CreateNew(
            "Street",
            "City",
            "State",
            "Country",
            "ZipCode"
        );

        public static readonly IList<PermissionId> PermissionIds =
        [
            PermissionConstants.GetCategoryPermission.Id,
            PermissionConstants.GetProductPermission.Id,
        ];
    }
}
