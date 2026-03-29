using FluentValidation.TestHelper;
using Shopizy.Application.Wishlists.Commands.UpdateWishlist;

namespace Shopizy.Application.UnitTests.Wishlists.Commands.UpdateWishlist;

public class UpdateWishlistCommandValidatorTests
{
    private readonly UpdateWishlistCommandValidator _validator = new();

    private static UpdateWishlistCommand ValidCommand() =>
        new(Guid.NewGuid(), Guid.NewGuid(), WishlistAction.Add);

    [Fact]
    public async Task Should_HaveError_When_UserIdIsEmpty()
    {
        var command = ValidCommand() with { UserId = Guid.Empty };
        var result = await _validator.TestValidateAsync(command, cancellationToken: TestContext.Current.CancellationToken);
        result.ShouldHaveValidationErrorFor(x => x.UserId);
    }

    [Fact]
    public async Task Should_HaveError_When_ProductIdIsEmpty()
    {
        var command = ValidCommand() with { ProductId = Guid.Empty };
        var result = await _validator.TestValidateAsync(command, cancellationToken: TestContext.Current.CancellationToken);
        result.ShouldHaveValidationErrorFor(x => x.ProductId);
    }

    [Fact]
    public async Task Should_HaveError_When_ActionIsInvalidEnumValue()
    {
        var command = ValidCommand() with { Action = (WishlistAction)99 };
        var result = await _validator.TestValidateAsync(command, cancellationToken: TestContext.Current.CancellationToken);
        result.ShouldHaveValidationErrorFor(x => x.Action);
    }

    [Theory]
    [InlineData(WishlistAction.Add)]
    [InlineData(WishlistAction.Remove)]
    public async Task Should_NotHaveErrors_When_AllFieldsAreValid(WishlistAction action)
    {
        var command = ValidCommand() with { Action = action };
        var result = await _validator.TestValidateAsync(command, cancellationToken: TestContext.Current.CancellationToken);
        result.ShouldNotHaveAnyValidationErrors();
    }
}
