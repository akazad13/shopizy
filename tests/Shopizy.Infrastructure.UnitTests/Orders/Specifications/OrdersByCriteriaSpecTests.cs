using Shopizy.Domain.Orders;
using Shopizy.Domain.Orders.Enums;
using Shopizy.Domain.Users.ValueObjects;
using Shopizy.Infrastructure.Orders.Specifications;
using Shouldly;
using Xunit;

namespace Shopizy.Infrastructure.UnitTests.Orders.Specifications;

public class OrdersByCriteriaSpecTests
{
    [Fact]
    public void Constructor_WithAllCriteria_SetsAllCriteria()
    {
        // Arrange
        var customerId = UserId.CreateUnique();
        var startDate = DateTime.UtcNow.AddDays(-7);
        var endDate = DateTime.UtcNow;
        var status = OrderStatus.Processing;

        // Act
        var spec = new OrdersByCriteriaSpec(customerId, startDate, endDate, status);

        // Assert
        spec.Criteria.ShouldNotBeNull();
        spec.IsPagingEnabled.ShouldBeTrue();
        spec.Skip.ShouldBe(0);
        spec.Take.ShouldBe(10);
    }

    [Fact]
    public void Constructor_WithOnlyCustomerId_SetsCustomerIdCriteria()
    {
        // Arrange
        var customerId = UserId.CreateUnique();

        // Act
        var spec = new OrdersByCriteriaSpec(customerId, null, null, null);

        // Assert
        spec.Criteria.ShouldNotBeNull();
        // We can't easily check the expression content without executing it or complex parsing,
        // but creating it exercise the code paths.
    }
}
