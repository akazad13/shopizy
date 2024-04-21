using Shopizy.Domain.Users.ValueObjects;

namespace Shopizy.Application.UnitTests.TestUtils.Constants;

public static partial class Constants
{
    public static class User
    {
        public static readonly UserId Id = UserId.Create(
            new Guid("88d9af82-bef4-4042-8e55-b01b89f23e68")
        );
    }

    public static class Address
    {
        public const string Line = "Line";
        public const string City = "City";
        public const string State = "State";
        public const string Country = "Country";

        public const string ZipCode = "ZipCode";
    }
}
