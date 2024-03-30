using Shopizy.Domain.Customers.ValueObjects;

namespace Shopizy.Application.UnitTests.TestUtils.Constants;

public static partial class Constants
{
    public static class Customer
    {
        public static readonly CustomerId Id = CustomerId.Create(
            new Guid("dd0aa32a-f7ab-4d48-b33e-1a3c1092f1e2")
        );
    }
}
