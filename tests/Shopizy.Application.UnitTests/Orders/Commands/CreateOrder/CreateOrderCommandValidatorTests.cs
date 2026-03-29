using FluentValidation.TestHelper;
using Shopizy.Application.Orders.Commands.CreateOrder;
using Shopizy.Domain.Common.Enums;
using Shouldly;

namespace Shopizy.Application.UnitTests.Orders.Commands.CreateOrder;

public class CreateOrderCommandValidatorTests
{
    private readonly CreateOrderCommandValidator _validator = new();

    private static AddressCommand ValidAddress() =>
        new("123 Main St", "Springfield", "IL", "USA", "62701");

    private static OrderItemCommand ValidItem() =>
        new(Guid.NewGuid(), "Red", "M", 2);

    private static CreateOrderCommand ValidCommand() => new(
        Guid.NewGuid(), "", 1, 5.99m, Currency.usd, [ValidItem()], ValidAddress());

    [Fact]
    public async Task Should_HaveError_When_UserIdIsEmpty()
    {
        var command = ValidCommand() with { UserId = Guid.Empty };
        var result = await _validator.TestValidateAsync(command, cancellationToken: TestContext.Current.CancellationToken);
        result.ShouldHaveValidationErrorFor(x => x.UserId);
    }

    [Fact]
    public async Task Should_HaveError_When_DeliveryChargeAmountIsNegative()
    {
        var command = ValidCommand() with { DeliveryChargeAmount = -0.01m };
        var result = await _validator.TestValidateAsync(command, cancellationToken: TestContext.Current.CancellationToken);
        result.ShouldHaveValidationErrorFor(x => x.DeliveryChargeAmount);
    }

    [Fact]
    public async Task Should_NotHaveError_When_DeliveryChargeAmountIsZero()
    {
        var command = ValidCommand() with { DeliveryChargeAmount = 0 };
        var result = await _validator.TestValidateAsync(command, cancellationToken: TestContext.Current.CancellationToken);
        result.ShouldNotHaveValidationErrorFor(x => x.DeliveryChargeAmount);
    }

    [Fact]
    public async Task Should_HaveError_When_OrderItemsIsEmpty()
    {
        var command = ValidCommand() with { OrderItems = [] };
        var result = await _validator.TestValidateAsync(command, cancellationToken: TestContext.Current.CancellationToken);
        result.ShouldHaveValidationErrorFor(x => x.OrderItems);
    }

    [Fact]
    public async Task Should_HaveError_When_OrderItemProductIdIsEmpty()
    {
        var invalidItem = new OrderItemCommand(Guid.Empty, "Red", "M", 1);
        var command = ValidCommand() with { OrderItems = [invalidItem] };
        var result = await _validator.TestValidateAsync(command, cancellationToken: TestContext.Current.CancellationToken);
        result.Errors.ShouldContain(e => e.PropertyName.Contains("ProductId"));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public async Task Should_HaveError_When_OrderItemQuantityIsNotPositive(int quantity)
    {
        var invalidItem = new OrderItemCommand(Guid.NewGuid(), "Red", "M", quantity);
        var command = ValidCommand() with { OrderItems = [invalidItem] };
        var result = await _validator.TestValidateAsync(command, cancellationToken: TestContext.Current.CancellationToken);
        result.Errors.ShouldContain(e => e.PropertyName.Contains("Quantity"));
    }

    [Fact]
    public async Task Should_HaveError_When_ShippingAddressStreetIsEmpty()
    {
        var command = ValidCommand() with { ShippingAddress = ValidAddress() with { Street = "" } };
        var result = await _validator.TestValidateAsync(command, cancellationToken: TestContext.Current.CancellationToken);
        result.Errors.ShouldContain(e => e.PropertyName.Contains("Street"));
    }

    [Fact]
    public async Task Should_HaveError_When_ShippingAddressCityIsEmpty()
    {
        var command = ValidCommand() with { ShippingAddress = ValidAddress() with { City = "" } };
        var result = await _validator.TestValidateAsync(command, cancellationToken: TestContext.Current.CancellationToken);
        result.Errors.ShouldContain(e => e.PropertyName.Contains("City"));
    }

    [Fact]
    public async Task Should_HaveError_When_ShippingAddressZipCodeExceedsMaxLength()
    {
        var command = ValidCommand() with { ShippingAddress = ValidAddress() with { ZipCode = new string('1', 11) } };
        var result = await _validator.TestValidateAsync(command, cancellationToken: TestContext.Current.CancellationToken);
        result.Errors.ShouldContain(e => e.PropertyName.Contains("ZipCode"));
    }

    [Fact]
    public async Task Should_NotHaveErrors_When_AllFieldsAreValid()
    {
        var command = ValidCommand();
        var result = await _validator.TestValidateAsync(command, cancellationToken: TestContext.Current.CancellationToken);
        result.ShouldNotHaveAnyValidationErrors();
    }
}
