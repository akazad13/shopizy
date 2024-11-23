using ErrorOr;
using FluentAssertions;
using Moq;
using Shopizy.Application.Common.Interfaces.Persistence;
using Shopizy.Application.Products.Commands.UpdateProduct;
using Shopizy.Application.UnitTests.Products.TestUtils;
using Shopizy.Domain.Products;
using Shopizy.Domain.Products.ValueObjects;

namespace Shopizy.Application.UnitTests.Products.Commands.UpdateProduct;

public class UpdateProductCommandHandlerTests
{
    private readonly UpdateProductCommandHandler _sut;
    private readonly Mock<IProductRepository> _mockProductRepository;

    public UpdateProductCommandHandlerTests()
    {
        _mockProductRepository = new Mock<IProductRepository>();
        _sut = new UpdateProductCommandHandler(_mockProductRepository.Object);
    }

    [Fact]
    public async Task ShouldUpdateTheProductSuccessfullyWhenAllRequiredFieldsAreProvidedAsync()
    {
        // Arrange
        var command = UpdateProductCommandUtils.CreateCommand();
        var product = ProductFactory.CreateProduct();

        _mockProductRepository
            .Setup(x => x.GetProductByIdAsync(It.IsAny<ProductId>()))
            .ReturnsAsync(product);

        _mockProductRepository.Setup(x => x.Update(It.IsAny<Product>()));
        _mockProductRepository.Setup(x => x.Commit(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.IsError.Should().BeFalse();
        result.Value.Should().BeOfType<Success>();

        _mockProductRepository.Verify(
            x => x.GetProductByIdAsync(It.IsAny<ProductId>()),
            Times.Once
        );
        _mockProductRepository.Verify(x => x.Update(It.IsAny<Product>()), Times.Once);
        _mockProductRepository.Verify(x => x.Commit(It.IsAny<CancellationToken>()), Times.Once);
    }

    // [Fact]
    // public async Task ShouldReturnError_WhenUpdatingProductWithInvalidProductId()
    // {
    //     // Arrange
    //     var invalidProductId = -1;
    //     var categoryId = CategoryId.Create(1);
    //     var product = new Product(
    //         ProductId.Create(invalidProductId),
    //         "Test Product",
    //         "Test Description",
    //         categoryId,
    //         "Test Sku",
    //         Price.CreateNew(10, "USD"),
    //         0,
    //         "Test Brand",
    //         "Test Barcode",
    //         new List<string>()
    //     );
    //     _mockProductRepository
    //         .Setup(x => x.GetProductByIdAsync(It.IsAny<ProductId>(), It.IsAny<CancellationToken>()))
    //         .ReturnsAsync(product);

    //     var command = new UpdateProductCommand
    //     {
    //         ProductId = invalidProductId,
    //         Name = "Updated Test Product",
    //         Description = "Updated Test Description",
    //         CategoryId = categoryId.Value,
    //         Sku = "Updated Test Sku",
    //         UnitPrice = 15,
    //         Currency = "USD",
    //         Discount = 5,
    //         Brand = "Updated Test Brand",
    //         Barcode = "Updated Test Barcode",
    //         Tags = new List<string> { "Updated Tag" },
    //     };

    //     var handler = new UpdateProductCommandHandler(_mockProductRepository.Object);

    //     // Act
    //     var result = await handler.Handle(command, CancellationToken.None);

    //     // Assert
    //     Assert.False(result.IsSuccess);
    //     Assert.Equal(CustomErrors.Product.ProductNotFound, result.Errors.First());
    //     _mockProductRepository.Verify(
    //         x => x.GetProductByIdAsync(It.IsAny<ProductId>(), It.IsAny<CancellationToken>()),
    //         Times.Once
    //     );
    //     _mockProductRepository.Verify(x => x.Update(It.IsAny<Product>()), Times.Never);
    //     _mockProductRepository.Verify(x => x.Commit(It.IsAny<CancellationToken>()), Times.Never);
    // }

    // [Fact]
    // public async Task ShouldReturnError_WhenUpdatingProductWithNonExistingCategoryId()
    // {
    //     // Arrange
    //     var productId = ProductId.Create(1);
    //     var nonExistingCategoryId = CategoryId.Create(999999); // Assuming this category ID does not exist
    //     var product = new Product(
    //         productId,
    //         "Test Product",
    //         "Test Description",
    //         CategoryId.Create(1),
    //         "Test Sku",
    //         Price.CreateNew(10, "USD"),
    //         0,
    //         "Test Brand",
    //         "Test Barcode",
    //         new List<string>()
    //     );
    //     _mockProductRepository
    //         .Setup(x => x.GetProductByIdAsync(It.IsAny<ProductId>(), It.IsAny<CancellationToken>()))
    //         .ReturnsAsync(product);
    //     _mockProductRepository.Setup(x => x.Update(It.IsAny<Product>()));
    //     _mockProductRepository.Setup(x => x.Commit(It.IsAny<CancellationToken>())).ReturnsAsync(1);

    //     var command = new UpdateProductCommand
    //     {
    //         ProductId = productId.Value,
    //         Name = "Updated Test Product",
    //         Description = "Updated Test Description",
    //         CategoryId = nonExistingCategoryId.Value,
    //         Sku = "Updated Test Sku",
    //         UnitPrice = 15,
    //         Currency = "USD",
    //         Discount = 5,
    //         Brand = "Updated Test Brand",
    //         Barcode = "Updated Test Barcode",
    //         Tags = new List<string> { "Updated Tag" },
    //     };

    //     var handler = new UpdateProductCommandHandler(_mockProductRepository.Object);

    //     // Act
    //     var result = await handler.Handle(command, CancellationToken.None);

    //     // Assert
    //     Assert.False(result.IsSuccess);
    //     Assert.Equal(CustomErrors.Product.ProductNotFound, result.Errors.First());
    //     _mockProductRepository.Verify(
    //         x => x.GetProductByIdAsync(It.IsAny<ProductId>(), It.IsAny<CancellationToken>()),
    //         Times.Once
    //     );
    //     _mockProductRepository.Verify(x => x.Update(It.IsAny<Product>()), Times.Never);
    //     _mockProductRepository.Verify(x => x.Commit(It.IsAny<CancellationToken>()), Times.Never);
    // }

    // [Fact]
    // public async Task ShouldHandleConcurrentUpdatesToTheSameProductWithoutDataCorruption()
    // {
    //     // Arrange
    //     var productId = ProductId.Create(1);
    //     var categoryId = CategoryId.Create(1);
    //     var product = new Product(
    //         productId,
    //         "Test Product",
    //         "Test Description",
    //         categoryId,
    //         "Test Sku",
    //         Price.CreateNew(10, "USD"),
    //         0,
    //         "Test Brand",
    //         "Test Barcode",
    //         new List<string>()
    //     );
    //     _mockProductRepository
    //         .Setup(x => x.GetProductByIdAsync(It.IsAny<ProductId>(), It.IsAny<CancellationToken>()))
    //         .ReturnsAsync(product);
    //     _mockProductRepository.Setup(x => x.Update(It.IsAny<Product>()));
    //     _mockProductRepository.Setup(x => x.Commit(It.IsAny<CancellationToken>())).ReturnsAsync(1);

    //     var command1 = new UpdateProductCommand
    //     {
    //         ProductId = productId.Value,
    //         Name = "Updated Test Product 1",
    //         // Other fields...
    //     };
    //     var command2 = new UpdateProductCommand
    //     {
    //         ProductId = productId.Value,
    //         Name = "Updated Test Product 2",
    //         // Other fields...
    //     };

    //     var handler = new UpdateProductCommandHandler(_mockProductRepository.Object);

    //     // Act
    //     var task1 = handler.Handle(command1, CancellationToken.None);
    //     var task2 = handler.Handle(command2, CancellationToken.None);

    //     await Task.WhenAll(task1, task2);

    //     // Assert
    //     Assert.True(task1.Result.IsSuccess);
    //     Assert.True(task2.Result.IsSuccess);
    //     Assert.Equal("Updated Test Product 2", task2.Result.Value.Name);
    //     _mockProductRepository.Verify(
    //         x => x.GetProductByIdAsync(It.IsAny<ProductId>(), It.IsAny<CancellationToken>()),
    //         Times.Exactly(2)
    //     );
    //     _mockProductRepository.Verify(x => x.Update(It.IsAny<Product>()), Times.Exactly(2));
    //     _mockProductRepository.Verify(
    //         x => x.Commit(It.IsAny<CancellationToken>()),
    //         Times.Exactly(2)
    //     );
    // }

    // [Fact]
    // public async Task ShouldReturnError_WhenUpdatingProductWithNegativeUnitPrice()
    // {
    //     // Arrange
    //     var productId = ProductId.Create(1);
    //     var categoryId = CategoryId.Create(1);
    //     var product = new Product(
    //         productId,
    //         "Test Product",
    //         "Test Description",
    //         categoryId,
    //         "Test Sku",
    //         Price.CreateNew(10, "USD"),
    //         0,
    //         "Test Brand",
    //         "Test Barcode",
    //         new List<string>()
    //     );
    //     _mockProductRepository
    //         .Setup(x => x.GetProductByIdAsync(It.IsAny<ProductId>(), It.IsAny<CancellationToken>()))
    //         .ReturnsAsync(product);

    //     var command = new UpdateProductCommand
    //     {
    //         ProductId = productId.Value,
    //         Name = "Updated Test Product",
    //         Description = "Updated Test Description",
    //         CategoryId = categoryId.Value,
    //         Sku = "Updated Test Sku",
    //         UnitPrice = -15, // Negative unit price
    //         Currency = "USD",
    //         Discount = 5,
    //         Brand = "Updated Test Brand",
    //         Barcode = "Updated Test Barcode",
    //         Tags = new List<string> { "Updated Tag" },
    //     };

    //     var handler = new UpdateProductCommandHandler(_mockProductRepository.Object);

    //     // Act
    //     var result = await handler.Handle(command, CancellationToken.None);

    //     // Assert
    //     Assert.False(result.IsSuccess);
    //     Assert.Equal(CustomErrors.Product.InvalidUnitPrice, result.Errors.First());
    //     _mockProductRepository.Verify(
    //         x => x.GetProductByIdAsync(It.IsAny<ProductId>(), It.IsAny<CancellationToken>()),
    //         Times.Once
    //     );
    //     _mockProductRepository.Verify(x => x.Update(It.IsAny<Product>()), Times.Never);
    //     _mockProductRepository.Verify(x => x.Commit(It.IsAny<CancellationToken>()), Times.Never);
    // }

    // [Fact]
    // public async Task ShouldReturnError_WhenUpdatingProductWithInvalidCurrencyCode()
    // {
    //     // Arrange
    //     var productId = ProductId.Create(1);
    //     var categoryId = CategoryId.Create(1);
    //     var product = new Product(
    //         productId,
    //         "Test Product",
    //         "Test Description",
    //         categoryId,
    //         "Test Sku",
    //         Price.CreateNew(10, "USD"),
    //         0,
    //         "Test Brand",
    //         "Test Barcode",
    //         new List<string>()
    //     );
    //     _mockProductRepository
    //         .Setup(x => x.GetProductByIdAsync(It.IsAny<ProductId>(), It.IsAny<CancellationToken>()))
    //         .ReturnsAsync(product);
    //     _mockProductRepository.Setup(x => x.Update(It.IsAny<Product>()));
    //     _mockProductRepository.Setup(x => x.Commit(It.IsAny<CancellationToken>())).ReturnsAsync(1);

    //     var command = new UpdateProductCommand
    //     {
    //         ProductId = productId.Value,
    //         Name = "Updated Test Product",
    //         Description = "Updated Test Description",
    //         CategoryId = categoryId.Value,
    //         Sku = "Updated Test Sku",
    //         UnitPrice = 15,
    //         Currency = "XYZ", // Invalid currency code
    //         Discount = 5,
    //         Brand = "Updated Test Brand",
    //         Barcode = "Updated Test Barcode",
    //         Tags = new List<string> { "Updated Tag" },
    //     };

    //     var handler = new UpdateProductCommandHandler(_mockProductRepository.Object);

    //     // Act
    //     var result = await handler.Handle(command, CancellationToken.None);

    //     // Assert
    //     Assert.False(result.IsSuccess);
    //     Assert.Equal(CustomErrors.Product.InvalidCurrencyCode, result.Errors.Single());
    //     _mockProductRepository.Verify(
    //         x => x.GetProductByIdAsync(It.IsAny<ProductId>(), It.IsAny<CancellationToken>()),
    //         Times.Once
    //     );
    //     _mockProductRepository.Verify(x => x.Update(It.IsAny<Product>()), Times.Never);
    //     _mockProductRepository.Verify(x => x.Commit(It.IsAny<CancellationToken>()), Times.Never);
    // }

    // [Fact]
    // public async Task ShouldReturnError_WhenUpdatingProductWithDiscountGreaterThan100Percent()
    // {
    //     // Arrange
    //     var productId = ProductId.Create(1);
    //     var categoryId = CategoryId.Create(1);
    //     var product = new Product(
    //         productId,
    //         "Test Product",
    //         "Test Description",
    //         categoryId,
    //         "Test Sku",
    //         Price.CreateNew(10, "USD"),
    //         0,
    //         "Test Brand",
    //         "Test Barcode",
    //         new List<string>()
    //     );
    //     _mockProductRepository
    //         .Setup(x => x.GetProductByIdAsync(It.IsAny<ProductId>(), It.IsAny<CancellationToken>()))
    //         .ReturnsAsync(product);
    //     _mockProductRepository.Setup(x => x.Update(It.IsAny<Product>()));
    //     _mockProductRepository.Setup(x => x.Commit(It.IsAny<CancellationToken>())).ReturnsAsync(1);

    //     var command = new UpdateProductCommand
    //     {
    //         ProductId = productId.Value,
    //         Name = "Updated Test Product",
    //         Description = "Updated Test Description",
    //         CategoryId = categoryId.Value,
    //         Sku = "Updated Test Sku",
    //         UnitPrice = 15,
    //         Currency = "USD",
    //         Discount = 101, // Invalid discount value
    //         Brand = "Updated Test Brand",
    //         Barcode = "Updated Test Barcode",
    //         Tags = new List<string> { "Updated Tag" },
    //     };

    //     var handler = new UpdateProductCommandHandler(_mockProductRepository.Object);

    //     // Act
    //     var result = await handler.Handle(command, CancellationToken.None);

    //     // Assert
    //     Assert.False(result.IsSuccess);
    //     Assert.Equal(CustomErrors.Product.ProductDiscountInvalid, result.Errors.First());
    //     _mockProductRepository.Verify(
    //         x => x.GetProductByIdAsync(It.IsAny<ProductId>(), It.IsAny<CancellationToken>()),
    //         Times.Once
    //     );
    //     _mockProductRepository.Verify(x => x.Update(It.IsAny<Product>()), Times.Never);
    //     _mockProductRepository.Verify(x => x.Commit(It.IsAny<CancellationToken>()), Times.Never);
    // }

    // [Fact]
    // public async Task ShouldReturnError_WhenProductRepositoryFailsToCommitChanges()
    // {
    //     // Arrange
    //     var productId = ProductId.Create(1);
    //     var categoryId = CategoryId.Create(1);
    //     var product = new Product(
    //         productId,
    //         "Test Product",
    //         "Test Description",
    //         categoryId,
    //         "Test Sku",
    //         Price.CreateNew(10, "USD"),
    //         0,
    //         "Test Brand",
    //         "Test Barcode",
    //         new List<string>()
    //     );
    //     _mockProductRepository
    //         .Setup(x => x.GetProductByIdAsync(It.IsAny<ProductId>(), It.IsAny<CancellationToken>()))
    //         .ReturnsAsync(product);
    //     _mockProductRepository.Setup(x => x.Update(It.IsAny<Product>()));
    //     _mockProductRepository.Setup(x => x.Commit(It.IsAny<CancellationToken>())).ReturnsAsync(0);

    //     var command = new UpdateProductCommand
    //     {
    //         ProductId = productId.Value,
    //         Name = "Updated Test Product",
    //         Description = "Updated Test Description",
    //         CategoryId = categoryId.Value,
    //         Sku = "Updated Test Sku",
    //         UnitPrice = 15,
    //         Currency = "USD",
    //         Discount = 5,
    //         Brand = "Updated Test Brand",
    //         Barcode = "Updated Test Barcode",
    //         Tags = new List<string> { "Updated Tag" },
    //     };

    //     var handler = new UpdateProductCommandHandler(_mockProductRepository.Object);

    //     // Act
    //     var result = await handler.Handle(command, CancellationToken.None);

    //     // Assert
    //     Assert.False(result.IsSuccess);
    //     Assert.Equal(CustomErrors.Product.ProductNotUpdated, result.Error.Errors.First());
    //     _mockProductRepository.Verify(
    //         x => x.GetProductByIdAsync(It.IsAny<ProductId>(), It.IsAny<CancellationToken>()),
    //         Times.Once
    //     );
    //     _mockProductRepository.Verify(x => x.Update(It.IsAny<Product>()), Times.Once);
    //     _mockProductRepository.Verify(x => x.Commit(It.IsAny<CancellationToken>()), Times.Once);
    // }

    // [Fact]
    // public async Task ShouldReturnTheUpdatedProduct_WhenTheUpdateIsSuccessful()
    // {
    //     // Arrange
    //     var productId = ProductId.Create(1);
    //     var categoryId = CategoryId.Create(1);
    //     var product = new Product(
    //         productId,
    //         "Test Product",
    //         "Test Description",
    //         categoryId,
    //         "Test Sku",
    //         Price.CreateNew(10, "USD"),
    //         0,
    //         "Test Brand",
    //         "Test Barcode",
    //         new List<string>()
    //     );
    //     _mockProductRepository
    //         .Setup(x => x.GetProductByIdAsync(It.IsAny<ProductId>(), It.IsAny<CancellationToken>()))
    //         .ReturnsAsync(product);
    //     _mockProductRepository.Setup(x => x.Update(It.IsAny<Product>()));
    //     _mockProductRepository.Setup(x => x.Commit(It.IsAny<CancellationToken>())).ReturnsAsync(1);

    //     var command = new UpdateProductCommand
    //     {
    //         ProductId = productId.Value,
    //         Name = "Updated Test Product",
    //         Description = "Updated Test Description",
    //         CategoryId = categoryId.Value,
    //         Sku = "Updated Test Sku",
    //         UnitPrice = 15,
    //         Currency = "USD",
    //         Discount = 5,
    //         Brand = "Updated Test Brand",
    //         Barcode = "Updated Test Barcode",
    //         Tags = new List<string> { "Updated Tag" },
    //     };

    //     var handler = new UpdateProductCommandHandler(_mockProductRepository.Object);

    //     // Act
    //     var result = await handler.Handle(command, CancellationToken.None);

    //     // Assert
    //     Assert.True(result.IsSuccess);
    //     Assert.Equal("Updated Test Product", result.Value.Name);
    //     _mockProductRepository.Verify(
    //         x => x.GetProductByIdAsync(It.IsAny<ProductId>(), It.IsAny<CancellationToken>()),
    //         Times.Once
    //     );
    //     _mockProductRepository.Verify(x => x.Update(It.IsAny<Product>()), Times.Once);
    //     _mockProductRepository.Verify(x => x.Commit(It.IsAny<CancellationToken>()), Times.Once);
    // }

    // [Fact]
    // public void ShouldValidateAndSanitizeInputToPreventSecurityVulnerabilities()
    // {
    //     // Arrange
    //     var productId = 1; // Assume this is sanitized and validated
    //     var categoryId = 1; // Assume this is sanitized and validated
    //     var name = "Test Product'; DROP TABLE Products; --"; // SQL Injection attempt
    //     var description = "Test Description";
    //     var sku = "Test Sku";
    //     var unitPrice = 10;
    //     var currency = "USD";
    //     var discount = 0;
    //     var brand = "Test Brand";
    //     var barcode = "Test Barcode";
    //     var tags = new List<string> { "Test Tag" };

    //     var productRepositoryMock = new Mock<IProductRepository>();
    //     productRepositoryMock
    //         .Setup(x => x.GetProductByIdAsync(It.IsAny<ProductId>(), It.IsAny<CancellationToken>()))
    //         .ReturnsAsync((Product)null);

    //     var command = new UpdateProductCommand
    //     {
    //         ProductId = productId,
    //         Name = name,
    //         Description = description,
    //         CategoryId = categoryId,
    //         Sku = sku,
    //         UnitPrice = unitPrice,
    //         Currency = currency,
    //         Discount = discount,
    //         Brand = brand,
    //         Barcode = barcode,
    //         Tags = tags,
    //     };

    //     var handler = new UpdateProductCommandHandler(productRepositoryMock.Object);

    //     // Act
    //     var result = handler.Handle(command, CancellationToken.None).Result;

    //     // Assert
    //     Assert.False(result.IsSuccess);
    //     Assert.Equal(CustomErrors.Product.ProductNotFound, result.Errors.Single());
    //     productRepositoryMock.Verify(
    //         x => x.GetProductByIdAsync(It.IsAny<ProductId>(), It.IsAny<CancellationToken>()),
    //         Times.Once
    //     );
    //     productRepositoryMock.Verify(x => x.Update(It.IsAny<Product>()), Times.Never);
    //     productRepositoryMock.Verify(x => x.Commit(It.IsAny<CancellationToken>()), Times.Never);
    // }
}
