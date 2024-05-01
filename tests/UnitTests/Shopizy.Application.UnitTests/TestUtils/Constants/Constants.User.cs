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

        public static readonly Address Address = Address.CreateNew(
            "Line",
            "City",
            "State",
            "Country",
            "ZipCode"
        );
    }
}
