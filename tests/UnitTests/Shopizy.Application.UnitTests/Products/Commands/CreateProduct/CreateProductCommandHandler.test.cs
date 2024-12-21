using FluentAssertions;
using Moq;
using Shopizy.Application.Common.Interfaces.Persistence;
using Shopizy.Application.Products.Commands.CreateProduct;
using Shopizy.Application.UnitTests.Products.TestUtils;
using Shopizy.Application.UnitTests.TestUtils.Extensions;
using Shopizy.Domain.Common.CustomErrors;
using Shopizy.Domain.Products;

namespace Shopizy.Application.UnitTests.Products.Commands.CreateProduct;

public class CreateProductCommandHandlerTests
{
    // T1: SUT (system under testing) - logical component we are testing
    // T2: Scenerio - What we are testing
    // T3: Expected Outcome - what we expect the logical component to do

    private readonly Mock<IProductRepository> _mockProductRepository;
    private readonly CreateProductCommandHandler _sut;

    public CreateProductCommandHandlerTests()
    {
        _mockProductRepository = new Mock<IProductRepository>();
        _sut = new CreateProductCommandHandler(_mockProductRepository.Object);
    }

    [Fact]
    public async Task ShouldThrowExceptionWhenProductNameIsEmpty()
    {
        // Arrange
        var command = CreateProductCommandUtils.CreateCommandWithEmptyProductName();

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.IsError.Should().BeTrue();
        result.Errors.Should().NotBeNullOrEmpty();
        result.Errors[0].Should().Be(CustomErrors.Product.ProductNotCreated);

        _mockProductRepository.Verify(m => m.AddAsync(It.IsAny<Product>()), Times.Once);
        _mockProductRepository.Verify(m => m.Commit(default), Times.Once);
    }

    [Fact]
    public async Task ShouldCreateAndReturnProductWhenProductIsValid()
    {
        // Arrange

        var command = CreateProductCommandUtils.CreateCommand();

        _mockProductRepository.Setup(p => p.AddAsync(It.IsAny<Product>()));
        _mockProductRepository.Setup(p => p.Commit(default)).ReturnsAsync(1);
        // Act
        var result = await _sut.Handle(command, default);

        // Assert
        result.IsError.Should().BeFalse();
        result.Value.Should().BeOfType<Product>();
        result.Value.Should().NotBeNull();
        result.Value.ValidateResult(command);

        _mockProductRepository.Verify(m => m.AddAsync(It.IsAny<Product>()), Times.Once);
        _mockProductRepository.Verify(m => m.Commit(default), Times.Once);
    }

    // [Fact]
    // public async Task ShouldThrowExceptionWhenProductDescriptionIsTooLong()
    // {
    //     // Arrange
    //     var createProductCommand = new CreateProductCommand
    //     {
    //         Name = "Test Product",
    //         Description = new string('a', 501), // Description is too long
    //         CategoryId = 1,
    //         Sku = "Test Sku",
    //         UnitPrice = 100,
    //         Currency = "USD",
    //         Discount = 0,
    //         Brand = "Test Brand",
    //         Barcode = "Test Barcode",
    //         Tags = new List<string> { "Test Tag" },
    //     };

    //     var createProductCommandHandler = new CreateProductCommandHandler(
    //         _productRepositoryMock.Object
    //     );

    //     // Act
    //     var result = await createProductCommandHandler.Handle(
    //         createProductCommand,
    //         CancellationToken.None
    //     );

    //     // Assert
    //     result.ShouldNotBeNull();
    //     result.IsSuccess.ShouldBeFalse();
    //     result.Errors.Count.ShouldBe(1);
    //     result.Errors[0].Code.ShouldBe(CustomErrors.Product.ProductNotCreated);
    // }

    // [Fact]
    // public async Task ShouldReturnErrorResponseWhenProductSKUAlreadyExists()
    // {
    //     // Arrange
    //     var existingProduct = Product.Create(
    //         "Existing Product",
    //         "Test Description",
    //         CategoryId.Create(1),
    //         "Test Sku",
    //         Price.CreateNew(100, "USD"),
    //         0,
    //         "Test Brand",
    //         "Test Barcode",
    //         new List<string> { "Test Tag" }
    //     );

    //     _productRepositoryMock
    //         .Setup(x => x.GetBySKUAsync("Test Sku", CancellationToken.None))
    //         .ReturnsAsync(existingProduct);

    //     var createProductCommand = new CreateProductCommand
    //     {
    //         Name = "New Product",
    //         Description = "Test Description",
    //         CategoryId = 1,
    //         Sku = "Test Sku",
    //         UnitPrice = 100,
    //         Currency = "USD",
    //         Discount = 0,
    //         Brand = "Test Brand",
    //         Barcode = "Test Barcode",
    //         Tags = new List<string> { "Test Tag" },
    //     };

    //     var createProductCommandHandler = new CreateProductCommandHandler(
    //         _productRepositoryMock.Object
    //     );

    //     // Act
    //     var result = await createProductCommandHandler.Handle(
    //         createProductCommand,
    //         CancellationToken.None
    //     );

    //     // Assert
    //     result.ShouldNotBeNull();
    //     result.IsSuccess.ShouldBeFalse();
    //     result.Errors.Count.ShouldBe(1);
    //     result.Errors[0].Code.ShouldBe(CustomErrors.Product.ProductNotCreated);
    // }

    // [Fact]
    // public async Task ShouldReturnErrorResponseWhenProductPriceIsNegative()
    // {
    //     // Arrange
    //     var createProductCommand = new CreateProductCommand
    //     {
    //         Name = "Test Product",
    //         Description = "Test Description",
    //         CategoryId = 1,
    //         Sku = "Test Sku",
    //         UnitPrice = -100, // Negative price
    //         Currency = "USD",
    //         Discount = 0,
    //         Brand = "Test Brand",
    //         Barcode = "Test Barcode",
    //         Tags = new List<string> { "Test Tag" },
    //     };

    //     var createProductCommandHandler = new CreateProductCommandHandler(
    //         _productRepositoryMock.Object
    //     );

    //     // Act
    //     var result = await createProductCommandHandler.Handle(
    //         createProductCommand,
    //         CancellationToken.None
    //     );

    //     // Assert
    //     result.ShouldNotBeNull();
    //     result.IsSuccess.ShouldBeFalse();
    //     result.Errors.Count.ShouldBe(1);
    //     result.Errors[0].Code.ShouldBe(CustomErrors.Product.InvalidPrice);
    // }

    // [Fact]
    // public async Task ShouldReturnErrorResponseWhenProductDiscountIsGreaterThan100Percent()
    // {
    //     // Arrange
    //     var createProductCommand = new CreateProductCommand
    //     {
    //         Name = "Test Product",
    //         Description = "Test Description",
    //         CategoryId = 1,
    //         Sku = "Test Sku",
    //         UnitPrice = 100,
    //         Currency = "USD",
    //         Discount = 101, // Invalid discount value
    //         Brand = "Test Brand",
    //         Barcode = "Test Barcode",
    //         Tags = new List<string> { "Test Tag" },
    //     };

    //     var createProductCommandHandler = new CreateProductCommandHandler(
    //         _productRepositoryMock.Object
    //     );

    //     // Act
    //     var result = await createProductCommandHandler.Handle(
    //         createProductCommand,
    //         CancellationToken.None
    //     );

    //     // Assert
    //     result.ShouldNotBeNull();
    //     result.IsSuccess.ShouldBeFalse();
    //     result.Errors.Count.ShouldBe(1);
    //     result.Errors[0].Code.ShouldBe(CustomErrors.Product.InvalidDiscount);
    // }

    // [Fact]
    // public async Task ShouldHandleConcurrentRequestsWithoutDataCorruption()
    // {
    //     // Arrange
    //     var createProductCommand = new CreateProductCommand
    //     {
    //         Name = "Test Product",
    //         Description = "Test Description",
    //         CategoryId = 1,
    //         Sku = "Test Sku",
    //         UnitPrice = 100,
    //         Currency = "USD",
    //         Discount = 0,
    //         Brand = "Test Brand",
    //         Barcode = "Test Barcode",
    //         Tags = new List<string> { "Test Tag" },
    //     };

    //     var createProductCommandHandler = new CreateProductCommandHandler(
    //         _productRepositoryMock.Object
    //     );

    //     _productRepositoryMock
    //         .Setup(repo => repo.AddAsync(It.IsAny<Product>(), It.IsAny<CancellationToken>()))
    //         .Returns(Task.CompletedTask);

    //     _productRepositoryMock
    //         .Setup(repo => repo.Commit(It.IsAny<CancellationToken>()))
    //         .ReturnsAsync(1);

    //     // Act
    //     var tasks = new List<Task<ErrorOr<Product>>>();
    //     for (int i = 0; i < 10; i++)
    //     {
    //         tasks.Add(
    //             createProductCommandHandler.Handle(createProductCommand, CancellationToken.None)
    //         );
    //     }

    //     await Task.WhenAll(tasks);

    //     // Assert
    //     foreach (var task in tasks)
    //     {
    //         task.Result.ShouldNotBeNull();
    //         task.Result.IsSuccess.ShouldBeTrue();
    //         task.Result.Errors.ShouldBeEmpty();
    //     }

    //     _productRepositoryMock.Verify(
    //         repo => repo.AddAsync(It.IsAny<Product>(), It.IsAny<CancellationToken>()),
    //         Times.Exactly(10)
    //     );
    //     _productRepositoryMock.Verify(
    //         repo => repo.Commit(It.IsAny<CancellationToken>()),
    //         Times.Exactly(10)
    //     );
    // }

    // [Fact]
    // public async Task ShouldValidateProductBarcodeFormat()
    // {
    //     // Arrange
    //     var createProductCommand = new CreateProductCommand
    //     {
    //         Name = "Test Product",
    //         Description = "Test Description",
    //         CategoryId = 1,
    //         Sku = "Test Sku",
    //         UnitPrice = 100,
    //         Currency = "USD",
    //         Discount = 0,
    //         Brand = "Test Brand",
    //         Barcode = "1234567890", // Valid barcode format
    //         Tags = new List<string> { "Test Tag" },
    //     };

    //     var createProductCommandHandler = new CreateProductCommandHandler(
    //         _productRepositoryMock.Object
    //     );

    //     // Act
    //     var result = await createProductCommandHandler.Handle(
    //         createProductCommand,
    //         CancellationToken.None
    //     );

    //     // Assert
    //     result.ShouldNotBeNull();
    //     result.IsSuccess.ShouldBeTrue();
    //     result.Value.Barcode.ShouldBe(createProductCommand.Barcode);
    // }

    // [Fact]
    // public async Task ShouldSupportAddingMultipleTagsToProduct()
    // {
    //     // Arrange
    //     var createProductCommand = new CreateProductCommand
    //     {
    //         Name = "Test Product",
    //         Description = "Test Description",
    //         CategoryId = 1,
    //         Sku = "Test Sku",
    //         UnitPrice = 100,
    //         Currency = "USD",
    //         Discount = 0,
    //         Brand = "Test Brand",
    //         Barcode = "Test Barcode",
    //         Tags = new List<string> { "Tag1", "Tag2", "Tag3" },
    //     };

    //     var productRepositoryMock = new Mock<IProductRepository>();
    //     var createProductCommandHandler = new CreateProductCommandHandler(
    //         productRepositoryMock.Object
    //     );

    //     // Act
    //     var result = await createProductCommandHandler.Handle(
    //         createProductCommand,
    //         CancellationToken.None
    //     );

    //     // Assert
    //     result.ShouldNotBeNull();
    //     result.IsSuccess.ShouldBeTrue();
    //     result.Value.Tags.Count.ShouldBe(3);
    //     result.Value.Tags.ShouldContain("Tag1");
    //     result.Value.Tags.ShouldContain("Tag2");
    //     result.Value.Tags.ShouldContain("Tag3");
    // }

    // [Fact]
    // public async Task ShouldHandleProductRepositoryUnavailable()
    // {
    //     // Arrange
    //     var createProductCommand = new CreateProductCommand
    //     {
    //         Name = "Test Product",
    //         Description = "Test Description",
    //         CategoryId = 1,
    //         Sku = "Test Sku",
    //         UnitPrice = 100,
    //         Currency = "USD",
    //         Discount = 0,
    //         Brand = "Test Brand",
    //         Barcode = "Test Barcode",
    //         Tags = new List<string> { "Test Tag" },
    //     };

    //     _productRepositoryMock
    //         .Setup(x => x.AddAsync(It.IsAny<Product>()))
    //         .ThrowsAsync(new Exception("Product repository unavailable"));

    //     var createProductCommandHandler = new CreateProductCommandHandler(
    //         _productRepositoryMock.Object
    //     );

    //     // Act
    //     var result = await createProductCommandHandler.Handle(
    //         createProductCommand,
    //         CancellationToken.None
    //     );

    //     // Assert
    //     result.ShouldNotBeNull();
    //     result.IsSuccess.ShouldBeFalse();
    //     result.Errors.Count.ShouldBe(1);
    //     result.Errors[0].Code.ShouldBe(CustomErrors.Product.ProductNotCreated);
    // }

    // [Fact]
    // public async Task ShouldReturnSuccessResponseWhenAllValidationsPassAndProductIsCreated()
    // {
    //     // Arrange
    //     var createProductCommand = new CreateProductCommand
    //     {
    //         Name = "Test Product",
    //         Description = "Test Description",
    //         CategoryId = 1,
    //         Sku = "Test Sku",
    //         UnitPrice = 100,
    //         Currency = "USD",
    //         Discount = 0,
    //         Brand = "Test Brand",
    //         Barcode = "Test Barcode",
    //         Tags = new List<string> { "Test Tag" },
    //     };

    //     var mockProductRepository = new Mock<IProductRepository>();
    //     mockProductRepository
    //         .Setup(repo => repo.AddAsync(It.IsAny<Product>()))
    //         .Returns(Task.CompletedTask);
    //     mockProductRepository
    //         .Setup(repo => repo.Commit(It.IsAny<CancellationToken>()))
    //         .ReturnsAsync(1);

    //     var createProductCommandHandler = new CreateProductCommandHandler(
    //         mockProductRepository.Object
    //     );

    //     // Act
    //     var result = await createProductCommandHandler.Handle(
    //         createProductCommand,
    //         CancellationToken.None
    //     );

    //     // Assert
    //     result.ShouldNotBeNull();
    //     result.IsSuccess.ShouldBeTrue();
    //     result.Errors.Count.ShouldBe(0);
    //     result.Value.ShouldNotBeNull();
    //     result.Value.Name.ShouldBe(createProductCommand.Name);
    //     result.Value.Description.ShouldBe(createProductCommand.Description);
    //     result.Value.CategoryId.Value.ShouldBe(createProductCommand.CategoryId);
    //     result.Value.Sku.ShouldBe(createProductCommand.Sku);
    //     result.Value.UnitPrice.Value.ShouldBe(createProductCommand.UnitPrice);
    //     result.Value.UnitPrice.Currency.Code.ShouldBe(createProductCommand.Currency);
    //     result.Value.Discount.ShouldBe(createProductCommand.Discount);
    //     result.Value.Brand.ShouldBe(createProductCommand.Brand);
    //     result.Value.Barcode.ShouldBe(createProductCommand.Barcode);
    //     result.Value.Tags.Count.ShouldBe(createProductCommand.Tags.Count);
    //     result.Value.Tags.ShouldContain(tag => createProductCommand.Tags.Contains(tag));
    // }
}
