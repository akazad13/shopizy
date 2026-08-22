using System.Reflection;
using Shopizy.Domain.Customers;
using Shopizy.Domain.Customers.ValueObjects;
using Shopizy.Domain.Orders.ValueObjects;
using Shouldly;
using Xunit;

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
    public void Create_WithNullProfileImageUrl_ReturnsCustomerWithNullImage()
    {
        // Arrange
        var address = Address.CreateNew("Street", "City", "State", "Country", "12345");

        // Act
        var customer = Customer.Create(null, address);

        // Assert
        customer.ShouldNotBeNull();
        customer.ProfileImageUrl.ShouldBeNull();
        customer.Address.ShouldBe(address);
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

    [Fact]
    public void Customer_ParameterlessConstructor_CreatesInstanceForEFCore()
    {
        // Act
        var constructor = typeof(Customer).GetConstructor(
            BindingFlags.Instance | BindingFlags.NonPublic,
            null,
            Type.EmptyTypes,
            null
        );

        var instance = (Customer?)constructor?.Invoke(null);

        // Assert
        instance.ShouldNotBeNull();
    }

    [Fact]
    public void ModifiedOn_PrivateSetter_CanBeSet()
    {
        // Arrange
        var customer = Customer.Create(
            null,
            Address.CreateNew("Street", "City", "State", "Country", "12345")
        );
        var modifiedOn = DateTime.UtcNow;

        // Act
        var propertyInfo = typeof(Customer).GetProperty(nameof(Customer.ModifiedOn));
        propertyInfo?.SetValue(customer, modifiedOn);

        // Assert
        customer.ModifiedOn.ShouldBe(modifiedOn);
    }

    [Fact]
    public void CustomerId_Create_ShouldInitializeWithValue()
    {
        var rawGuid = Guid.NewGuid();
        var id = CustomerId.Create(rawGuid);

        id.Value.ShouldBe(rawGuid);
        id.GetEqualityComponents().ShouldContain(rawGuid);
    }

    [Fact]
    public void CustomerId_CreateUnique_ShouldGenerateNewUniqueId()
    {
        var id1 = CustomerId.CreateUnique();
        var id2 = CustomerId.CreateUnique();

        id1.Value.ShouldNotBe(Guid.Empty);
        id2.Value.ShouldNotBe(Guid.Empty);
        id1.ShouldNotBe(id2);
    }

    [Fact]
    public void CustomerId_Equality_ShouldBeEqual_WhenValuesAreSame()
    {
        var guid = Guid.NewGuid();
        var id1 = CustomerId.Create(guid);
        var id2 = CustomerId.Create(guid);

        id1.ShouldBe(id2);
        (id1 == id2).ShouldBeTrue();
    }
}
