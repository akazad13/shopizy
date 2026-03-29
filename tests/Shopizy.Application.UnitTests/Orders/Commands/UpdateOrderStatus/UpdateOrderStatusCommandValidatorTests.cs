using FluentValidation.TestHelper;
using Shopizy.Application.Orders.Commands.UpdateOrderStatus;
using Shopizy.Domain.Orders.Enums;

namespace Shopizy.Application.UnitTests.Orders.Commands.UpdateOrderStatus;

public class UpdateOrderStatusCommandValidatorTests
{
    private readonly UpdateOrderStatusCommandValidator _validator = new();

    [Fact]
    public async Task Should_HaveError_When_UserIdIsEmpty()
    {
        var command = new UpdateOrderStatusCommand(Guid.Empty, Guid.NewGuid(), OrderStatus.Processing);
        var result = await _validator.TestValidateAsync(command, cancellationToken: TestContext.Current.CancellationToken);
        result.ShouldHaveValidationErrorFor(x => x.UserId);
    }

    [Fact]
    public async Task Should_HaveError_When_OrderIdIsEmpty()
    {
        var command = new UpdateOrderStatusCommand(Guid.NewGuid(), Guid.Empty, OrderStatus.Processing);
        var result = await _validator.TestValidateAsync(command, cancellationToken: TestContext.Current.CancellationToken);
        result.ShouldHaveValidationErrorFor(x => x.OrderId);
    }

    [Fact]
    public async Task Should_HaveError_When_StatusIsInvalidEnumValue()
    {
        var command = new UpdateOrderStatusCommand(Guid.NewGuid(), Guid.NewGuid(), (OrderStatus)99);
        var result = await _validator.TestValidateAsync(command, cancellationToken: TestContext.Current.CancellationToken);
        result.ShouldHaveValidationErrorFor(x => x.Status);
    }

    [Theory]
    [InlineData(OrderStatus.Pending)]
    [InlineData(OrderStatus.Processing)]
    [InlineData(OrderStatus.Shipping)]
    [InlineData(OrderStatus.Delivered)]
    [InlineData(OrderStatus.Cancelled)]
    [InlineData(OrderStatus.Refunded)]
    public async Task Should_NotHaveErrors_When_StatusIsValidEnumValue(OrderStatus status)
    {
        var command = new UpdateOrderStatusCommand(Guid.NewGuid(), Guid.NewGuid(), status);
        var result = await _validator.TestValidateAsync(command, cancellationToken: TestContext.Current.CancellationToken);
        result.ShouldNotHaveAnyValidationErrors();
    }
}
