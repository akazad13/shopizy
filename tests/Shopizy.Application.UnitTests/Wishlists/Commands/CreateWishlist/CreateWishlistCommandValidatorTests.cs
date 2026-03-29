using FluentValidation.TestHelper;
using Shopizy.Application.Wishlists.Commands.CreateWishlist;

namespace Shopizy.Application.UnitTests.Wishlists.Commands.CreateWishlist;

public class CreateWishlistCommandValidatorTests
{
    private readonly CreateWishlistCommandValidator _validator = new();

    [Fact]
    public async Task Should_HaveError_When_UserIdIsEmpty()
    {
        var command = new CreateWishlistCommand(Guid.Empty);
        var result = await _validator.TestValidateAsync(command, cancellationToken: TestContext.Current.CancellationToken);
        result.ShouldHaveValidationErrorFor(x => x.UserId);
    }

    [Fact]
    public async Task Should_NotHaveErrors_When_UserIdIsValid()
    {
        var command = new CreateWishlistCommand(Guid.NewGuid());
        var result = await _validator.TestValidateAsync(command, cancellationToken: TestContext.Current.CancellationToken);
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public async Task Should_NotHaveErrors_When_OptionalFieldsAreProvided()
    {
        var command = new CreateWishlistCommand(Guid.NewGuid(), "My Wishlist", IsPublic: true);
        var result = await _validator.TestValidateAsync(command, cancellationToken: TestContext.Current.CancellationToken);
        result.ShouldNotHaveAnyValidationErrors();
    }
}
