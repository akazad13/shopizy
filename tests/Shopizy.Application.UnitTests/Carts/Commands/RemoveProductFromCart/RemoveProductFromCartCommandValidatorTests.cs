using FluentValidation.TestHelper;
using Shopizy.Application.Carts.Commands.RemoveProductFromCart;

namespace Shopizy.Application.UnitTests.Carts.Commands.RemoveProductFromCart;

public class RemoveProductFromCartCommandValidatorTests
{
    private readonly RemoveProductFromCartCommandValidator _validator = new();

    [Fact]
    public async Task Should_HaveError_When_UserIdIsEmpty()
    {
        var command = new RemoveProductFromCartCommand(Guid.Empty, Guid.NewGuid());
        var result = await _validator.TestValidateAsync(command, cancellationToken: TestContext.Current.CancellationToken);
        result.ShouldHaveValidationErrorFor(x => x.UserId);
    }

    [Fact]
    public async Task Should_HaveError_When_ItemIdIsEmpty()
    {
        var command = new RemoveProductFromCartCommand(Guid.NewGuid(), Guid.Empty);
        var result = await _validator.TestValidateAsync(command, cancellationToken: TestContext.Current.CancellationToken);
        result.ShouldHaveValidationErrorFor(x => x.ItemId);
    }

    [Fact]
    public async Task Should_NotHaveErrors_When_AllFieldsAreValid()
    {
        var command = new RemoveProductFromCartCommand(Guid.NewGuid(), Guid.NewGuid());
        var result = await _validator.TestValidateAsync(command, cancellationToken: TestContext.Current.CancellationToken);
        result.ShouldNotHaveAnyValidationErrors();
    }
}
