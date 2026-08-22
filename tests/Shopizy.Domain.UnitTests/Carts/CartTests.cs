using Shopizy.Domain.Carts.Entities;
using Shopizy.Domain.Products.ValueObjects;
using Shopizy.Domain.Users.ValueObjects;
using Shouldly;
using Xunit;

namespace Shopizy.Domain.UnitTests.Carts;

public class CartTests
{
    [Fact]
    public void Create_ShouldInitializeEmptyCart()
    {
        // Arrange
        var userId = UserId.CreateUnique();

        // Act
        var cart = Shopizy.Domain.Carts.Cart.Create(userId);

        // Assert
        cart.ShouldNotBeNull();
        cart.UserId.ShouldBe(userId);
        cart.CartItems.ShouldBeEmpty();
    }

    [Fact]
    public void AddLineItem_ShouldAddProductToCart()
    {
        // Arrange
        var cart = Shopizy.Domain.Carts.Cart.Create(UserId.CreateUnique());
        var productId = ProductId.CreateUnique();
        var quantity = 2;
        var item = CartItem.Create(productId, "Red", "M", quantity);

        // Act
        cart.AddLineItem(item);

        // Assert
        cart.CartItems.ShouldHaveSingleItem();
        cart.CartItems[0].ProductId.ShouldBe(productId);
        cart.CartItems[0].Quantity.ShouldBe(quantity);
    }

    [Fact]
    public void RemoveLineItem_ShouldRemoveProductFromCart()
    {
        // Arrange
        var cart = Shopizy.Domain.Carts.Cart.Create(UserId.CreateUnique());
        var item = CartItem.Create(ProductId.CreateUnique(), "Red", "M", 1);
        cart.AddLineItem(item);

        // Act
        cart.RemoveLineItem(item);

        // Assert
        cart.CartItems.ShouldBeEmpty();
    }

    [Fact]
    public void UpdateLineItem_ShouldUpdateQuantity()
    {
        // Arrange
        var cart = Shopizy.Domain.Carts.Cart.Create(UserId.CreateUnique());
        var item = CartItem.Create(ProductId.CreateUnique(), "Red", "M", 1);
        cart.AddLineItem(item);
        var newQuantity = 5;

        // Act
        cart.UpdateLineItem(item.Id, newQuantity);

        // Assert
        cart.CartItems[0].Quantity.ShouldBe(newQuantity);
    }

    [Fact]
    public void RecordAbandonedReminderSent_ShouldSetTimestamp_AndResetOnModification()
    {
        // Arrange
        var cart = Shopizy.Domain.Carts.Cart.Create(UserId.CreateUnique());
        var item = CartItem.Create(ProductId.CreateUnique(), "Red", "M", 1);
        cart.AddLineItem(item);

        var sentTime = DateTime.UtcNow;

        // Act & Assert 1: Record reminder sent
        cart.RecordAbandonedReminderSent(sentTime);
        cart.LastAbandonedReminderSentOn.ShouldBe(sentTime);

        // Act & Assert 2: Modifying item resets reminder timestamp
        cart.UpdateLineItem(item.Id, 3);
        cart.LastAbandonedReminderSentOn.ShouldBeNull();

        // Act & Assert 3: Recording again sets it, then adding item resets it
        cart.RecordAbandonedReminderSent(sentTime);
        cart.LastAbandonedReminderSentOn.ShouldBe(sentTime);

        cart.AddLineItem(CartItem.Create(ProductId.CreateUnique(), "Blue", "L", 1));
        cart.LastAbandonedReminderSentOn.ShouldBeNull();
    }
}
