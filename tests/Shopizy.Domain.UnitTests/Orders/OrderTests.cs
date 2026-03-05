using Xunit;
using Shouldly;
using Shopizy.Domain.Orders;
using Shopizy.Domain.Orders.ValueObjects;
using Shopizy.Domain.Orders.Entities;
using Shopizy.Domain.Orders.Enums;
using Shopizy.Domain.Orders.Events;
using Shopizy.Domain.Users.ValueObjects;
using Shopizy.Domain.Common.ValueObjects;
using Shopizy.Domain.Common.Enums;
using Shopizy.Domain.Payments.Enums;

namespace Shopizy.Domain.UnitTests.Orders;

public class OrderTests
{
    [Fact]
    public void Create_WithValidData_ShouldCreateOrderAndAddDomainEvent()
    {
        // Arrange
        var userId = UserId.CreateUnique();
        var promoCode = "PROMO2024";
        var deliveryMethod = (int)DeliveryMethods.Standard;
        var deliveryCharge = Price.CreateNew(10, Currency.usd);
        var shippingAddress = Address.CreateNew("Street", "City", "State", "Country", "12345");
        var orderItems = new List<OrderItem>
        {
            OrderItem.Create("Product 1", "url1", Price.CreateNew(50, Currency.usd), 2, "Red", "M", 10)
        };

        // Act
        var order = Order.Create(userId, promoCode, deliveryMethod, deliveryCharge, shippingAddress, orderItems);

        // Assert
        order.ShouldNotBeNull();
        order.UserId.ShouldBe(userId);
        order.OrderStatus.ShouldBe(OrderStatus.Pending);
        order.OrderItems.Count.ShouldBe(1);
        order.DomainEvents.ShouldContain(e => e is OrderCreatedDomainEvent);
    }

    [Fact]
    public void CancelOrder_ShouldChangeStatusAndAddDomainEvent()
    {
        // Arrange
        var order = CreateSampleOrder();
        var reason = "Changed my mind";

        // Act
        order.CancelOrder(reason);

        // Assert
        order.OrderStatus.ShouldBe(OrderStatus.Cancelled);
        order.CancellationReason.ShouldBe(reason);
        order.DomainEvents.ShouldContain(e => e is OrderCancelledDomainEvent);
    }

    [Fact]
    public void GetTotal_ShouldCalculateCorrectAmount()
    {
        // Arrange
        // Item 1: 50 * 2 = 100. Discount 10% = 10. Net = 90.
        // Delivery: 10.
        // Total = 100.
        var deliveryCharge = Price.CreateNew(10, Currency.usd);
        var orderItems = new List<OrderItem>
        {
            OrderItem.Create("Product 1", "url1", Price.CreateNew(50, Currency.usd), 2, "Red", "M", 10)
        };
        var order = Order.Create(UserId.CreateUnique(), "", (int)DeliveryMethods.Standard, deliveryCharge, 
            Address.CreateNew("S", "C", "ST", "CO", "Z"), orderItems);

        // Act
        var total = order.GetTotal();

        // Assert
        total.Amount.ShouldBe(100);
        total.Currency.ShouldBe(Currency.usd);
    }

    [Fact]
    public void UpdatePaymentStatus_ShouldUpdateStatus()
    {
        // Arrange
        var order = CreateSampleOrder();

        // Act
        order.UpdatePaymentStatus(PaymentStatus.Payed);

        // Assert
        order.PaymentStatus.ShouldBe(PaymentStatus.Payed);
    }

    private Order CreateSampleOrder()
    {
        return Order.Create(
            UserId.CreateUnique(),
            "",
            (int)DeliveryMethods.Standard,
            Price.CreateNew(0, Currency.usd),
            Address.CreateNew("S", "C", "ST", "CO", "Z"),
            new List<OrderItem>()
        );
    }
}
