using Shopizy.Domain.Orders.ValueObjects;
using Shouldly;
using Xunit;

namespace Shopizy.Domain.UnitTests.Orders;

public class AddressTests
{
    [Fact]
    public void CreateNew_ShouldInitializeAddressCorrectly()
    {
        var address1 = Address.CreateNew("123 Main St", "Springfield", "IL", "USA", "62701");
        var address2 = Address.CreateNew("123 Main St", "Springfield", "IL", "USA", "62701");
        var address3 = Address.CreateNew("456 Elm St", "Springfield", "IL", "USA", "62701");

        address1.Street.ShouldBe("123 Main St");
        address1.City.ShouldBe("Springfield");
        address1.State.ShouldBe("IL");
        address1.Country.ShouldBe("USA");
        address1.ZipCode.ShouldBe("62701");

        address1.ShouldBe(address2);
        address1.GetHashCode().ShouldBe(address2.GetHashCode());
        address1.ShouldNotBe(address3);
    }
}
