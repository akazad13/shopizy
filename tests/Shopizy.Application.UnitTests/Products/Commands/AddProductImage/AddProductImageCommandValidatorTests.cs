using FluentValidation.TestHelper;
using Microsoft.AspNetCore.Http;
using Moq;
using Shopizy.Application.Products.Commands.AddProductImage;

namespace Shopizy.Application.UnitTests.Products.Commands.AddProductImage;

public class AddProductImageCommandValidatorTests
{
    private readonly AddProductImageCommandValidator _validator;

    public AddProductImageCommandValidatorTests()
    {
        _validator = new AddProductImageCommandValidator();
    }

    [Fact]
    public async Task Should_HaveError_When_FileIsNull()
    {
        // Arrange
        var command = new AddProductImageCommand(
            Guid.NewGuid(),
            Guid.NewGuid(),
            null!
        );

        // Act
        var result = await _validator.TestValidateAsync(command, cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.File);
    }

    [Fact]
    public async Task Should_NotHaveError_When_FileIsProvided()
    {
        // Arrange
        var mockFile = new Mock<IFormFile>();
        mockFile.Setup(f => f.Length).Returns(1024);
        mockFile.Setup(f => f.FileName).Returns("test.jpg");

        var command = new AddProductImageCommand(
            Guid.NewGuid(),
            Guid.NewGuid(),
            mockFile.Object
        );

        // Act
        var result = await _validator.TestValidateAsync(command, cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.File);
    }

    [Fact]
    public async Task Should_NotHaveError_When_AllFieldsAreValid()
    {
        // Arrange
        var mockFile = new Mock<IFormFile>();
        mockFile.Setup(f => f.Length).Returns(2048);
        mockFile.Setup(f => f.FileName).Returns("product-image.png");
        mockFile.Setup(f => f.ContentType).Returns("image/png");

        var command = new AddProductImageCommand(
            Guid.NewGuid(),
            Guid.NewGuid(),
            mockFile.Object
        );

        // Act
        var result = await _validator.TestValidateAsync(command, cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }
}
