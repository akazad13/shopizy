using Shopizy.Domain.Orders.ValueObjects;
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
        public const string Phone = "1234567890";
        public const string Password = "password";
        public const string NewPassword = "password";
        public static readonly Address Address = Address.CreateNew(
            "Line",
            "City",
            "State",
            "Country",
            "ZipCode"
        );
    }
}
