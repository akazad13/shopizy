using Shouldly;
using Xunit;
using Shopizy.Domain.Customers;
using Shopizy.Domain.Orders.ValueObjects;

namespace Shopizy.Domain.UnitTests.Customers;

public class CustomerTests
{
    [Fact]
    public void Create_WithValidData_ReturnsCustomer()
    {
        // Arrange
        var profileImageUrl = "https://example.com/profile.jpg";
        var address = Address.CreateNew("Street", "City", "State", "Country", "12345");

        // Act
        var customer = Customer.Create(profileImageUrl, address);

        // Assert
        customer.ShouldNotBeNull();
        customer.Id.ShouldNotBeNull();
        customer.Id.Value.ShouldNotBe(Guid.Empty);
        customer.ProfileImageUrl.ShouldBe(profileImageUrl);
        customer.Address.ShouldBe(address);
        customer.CreatedOn.ShouldBe(default);
        customer.ModifiedOn.ShouldBeNull();
    }

    [Fact]
    public void SetAddress_UpdatesAddressProperty()
    {
        // Arrange
        var initialAddress = Address.CreateNew("Street1", "City1", "State1", "Country1", "11111");
        var newAddress = Address.CreateNew("Street2", "City2", "State2", "Country2", "22222");
        var customer = Customer.Create("image", initialAddress);

        // Act
        customer.Address = newAddress;

        // Assert
        customer.Address.ShouldBe(newAddress);
    }
}
