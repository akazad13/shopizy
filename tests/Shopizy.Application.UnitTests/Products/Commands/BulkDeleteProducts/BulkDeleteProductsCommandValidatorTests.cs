using FluentValidation.TestHelper;
using Shopizy.Application.Products.Commands.BulkDeleteProducts;

namespace Shopizy.Application.UnitTests.Products.Commands.BulkDeleteProducts;

public class BulkDeleteProductsCommandValidatorTests
{
    private readonly BulkDeleteProductsCommandValidator _validator = new();

    [Fact]
    public async Task Should_HaveError_When_ProductIdsIsEmpty()
    {
        var command = new BulkDeleteProductsCommand([]);
        var result = await _validator.TestValidateAsync(
            command,
            cancellationToken: TestContext.Current.CancellationToken
        );
        result.ShouldHaveValidationErrorFor(x => x.ProductIds);
    }

    [Fact]
    public async Task Should_HaveError_When_ProductIdsContainsEmptyGuid()
    {
        var command = new BulkDeleteProductsCommand([Guid.NewGuid(), Guid.Empty]);
        var result = await _validator.TestValidateAsync(
            command,
            cancellationToken: TestContext.Current.CancellationToken
        );
        result.ShouldHaveValidationErrorFor("ProductIds[1]");
    }

    [Fact]
    public async Task Should_NotHaveError_When_AllProductIdsAreValid()
    {
        var command = new BulkDeleteProductsCommand([Guid.NewGuid(), Guid.NewGuid()]);
        var result = await _validator.TestValidateAsync(
            command,
            cancellationToken: TestContext.Current.CancellationToken
        );
        result.ShouldNotHaveAnyValidationErrors();
    }
}
