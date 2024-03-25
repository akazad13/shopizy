using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Moq;
using Shopizy.Application.Common.Interfaces.Persistance;
using Shopizy.Application.Common.Interfaces.Services;
using Shopizy.Application.Products.Commands.CreateProduct;
using Shopizy.Application.Products.Common;
using Shopizy.Application.UnitTests.Products.Commands.TestUtils;
using Shopizy.Application.UnitTests.TestUtils.Products.Extensions;
using Shopizy.Domain.Categories.ValueObjects;
using Shopizy.Domain.Common.ValueObjects;
using Shopizy.Domain.Products;
using Shopizy.Domain.Products.Entities;

namespace Shopizy.Application.UnitTests.Products.Commands.CreateProduct;

public class CreateProductCommandHandlerTests
{
    // T1: SUT (system under testing) - logical component we are testing
    // T2: Scenerio - What we are testing
    // T3: Expected Outcome - what we expect the logical component to do

    private readonly CreateProductCommandHandler _handler;
    private readonly Mock<IProductRepository> _mockProductRepository;
    private readonly Mock<ICloudinaryMediaUploader> _mockCloudinaryMediaUploader;

    // T1_T2_T3
    public CreateProductCommandHandlerTests()
    {
        _mockProductRepository = new Mock<IProductRepository>();
        _mockCloudinaryMediaUploader = new Mock<ICloudinaryMediaUploader>();
        _handler = new CreateProductCommandHandler(
            _mockProductRepository.Object,
            _mockCloudinaryMediaUploader.Object
        );
    }

    // [Theory]
    // [MemberData(nameof(ValidCreateProductCommands))]
    [Fact]
    public async void CreateProduct_WhenProductIsValid_ShouldCreateAndReturnProduct()
    {
        // Arrange
        var expectedUrl = "https://res.cloudinary.com/test/image/upload/test";
        var expectedPublicId = "test";

        var createProductCommand = CreateProductCommandUtils.CreateCommand();
        var product = Product.Create(
            createProductCommand.Name,
            createProductCommand.Description,
            CategoryId.Create(createProductCommand.CategoryId),
            createProductCommand.Sku,
            createProductCommand.StockQuantity,
            Price.CreateNew(createProductCommand.UnitPrice, createProductCommand.Currency),
            createProductCommand.Discount,
            createProductCommand.Brand,
            createProductCommand.Barcode,
            createProductCommand.Tags,
            "",
            [ProductImage.Create(expectedUrl, 0, expectedPublicId), ProductImage.Create(expectedUrl, 0, expectedPublicId)]
        );

        _mockCloudinaryMediaUploader
            .Setup(cl => cl.UploadPhotoAsync(It.IsAny<IFormFile>(), default))
            .ReturnsAsync(new PhotoUploadResult(expectedUrl, expectedPublicId));

        _mockProductRepository.Setup(p => p.AddAsync(product));
        _mockProductRepository.Setup(p => p.Commit(default)).ReturnsAsync(1);
        // Act
        var result = await _handler.Handle(createProductCommand, default);

        // Assert
        result.IsError.Should().BeFalse();
        result.Value.ValidateCreatedForm(createProductCommand);

        _mockProductRepository.Verify(m => m.AddAsync(result.Value), Times.Once);
    }

    // [Theory]
    // [MemberData(nameof(ValidCreateProductCommands))]
    // public async void CreateProductCommand_WhenProductIsValid_ShouldCreateAndReturnProduct1(
    //     CreateProductCommand createProductCommand
    // )
    // {
    //     var product = Product.Create(
    //         createProductCommand.Name,
    //         createProductCommand.Description,
    //         createProductCommand.CategoryId,
    //         createProductCommand.Sku,
    //         createProductCommand.StockQuantity,
    //         Price.CreateNew(createProductCommand.UnitPrice, createProductCommand.Currency),
    //         createProductCommand.Discount,
    //         createProductCommand.Brand,
    //         createProductCommand.Barcode,
    //         createProductCommand.Tags,
    //         ""
    //     );

    //     _mockProductRepository.Setup(p => p.AddAsync(product));
    //     _mockProductRepository.Setup(p => p.Commit(default)).ReturnsAsync(1);
    //     //Act
    //     var result = await _handler.Handle(createProductCommand, default);

    //     //Assert
    //     result.IsError.Should().BeFalse();
    //     result.Value.ValidateCreatedForm(createProductCommand);

    //     _mockProductRepository.Verify(m => m.AddAsync(result.Value), Times.Once);
    // }

    // public static IEnumerable<object[]> ValidCreateProductCommands()
    // {
    //     yield return new[] { CreateProductCommandUtils.CreateCommand() };
    // }
}
