using FluentValidation.TestHelper;
using Shopizy.Application.Carts.Commands.UpdateProductQuantity;

namespace Shopizy.Application.UnitTests.Carts.Commands.UpdateProductQuantity;

public class UpdateProductQuantityCommandValidatorTests
{
    private readonly UpdateProductQuantityCommandValidator _validator = new();

    [Fact]
    public async Task Should_HaveError_When_UserIdIsEmpty()
    {
        var command = new UpdateProductQuantityCommand(Guid.Empty, Guid.NewGuid(), 1);
        var result = await _validator.TestValidateAsync(command, cancellationToken: TestContext.Current.CancellationToken);
        result.ShouldHaveValidationErrorFor(x => x.UserId);
    }

    [Fact]
    public async Task Should_HaveError_When_CartItemIdIsEmpty()
    {
        var command = new UpdateProductQuantityCommand(Guid.NewGuid(), Guid.Empty, 1);
        var result = await _validator.TestValidateAsync(command, cancellationToken: TestContext.Current.CancellationToken);
        result.ShouldHaveValidationErrorFor(x => x.CartItemId);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-5)]
    public async Task Should_HaveError_When_QuantityIsNotPositive(int quantity)
    {
        var command = new UpdateProductQuantityCommand(Guid.NewGuid(), Guid.NewGuid(), quantity);
        var result = await _validator.TestValidateAsync(command, cancellationToken: TestContext.Current.CancellationToken);
        result.ShouldHaveValidationErrorFor(x => x.Quantity);
    }

    [Fact]
    public async Task Should_NotHaveErrors_When_AllFieldsAreValid()
    {
        var command = new UpdateProductQuantityCommand(Guid.NewGuid(), Guid.NewGuid(), 3);
        var result = await _validator.TestValidateAsync(command, cancellationToken: TestContext.Current.CancellationToken);
        result.ShouldNotHaveAnyValidationErrors();
    }
}
