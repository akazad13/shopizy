using ErrorOr;
using FluentAssertions;
using Moq;
using Shopizy.Application.Common.Interfaces.Persistence;
using Shopizy.Application.Common.Interfaces.Services;
using Shopizy.Application.Products.Commands.DeleteProduct;
using Shopizy.Application.UnitTests.Products.TestUtils;
using Shopizy.Domain.Products;
using Shopizy.Domain.Products.ValueObjects;

namespace Shopizy.Application.UnitTests.Products.Commands.DeleteProduct;

public class DeleteProductCommandHandlerTests
{
    private readonly DeleteProductCommandHandler _sut;
    private readonly Mock<IProductRepository> _mockProductRepository;
    private readonly Mock<IMediaUploader> _mockMediaUploader;

    public DeleteProductCommandHandlerTests()
    {
        _mockProductRepository = new Mock<IProductRepository>();
        _mockMediaUploader = new Mock<IMediaUploader>();
        _sut = new DeleteProductCommandHandler(
            _mockProductRepository.Object,
            _mockMediaUploader.Object
        );
    }

    [Fact]
    public async Task Should_SuccessfullyDeleteAProduct_WithValidId()
    {
        // Arrange
        var command = DeleteProductCommandUtils.CreateCommand();
        var product = ProductFactory.CreateProduct();

        _mockProductRepository
            .Setup(p => p.GetProductByIdAsync(ProductId.Create(command.ProductId)))
            .ReturnsAsync(product);

        _mockProductRepository.Setup(p => p.Remove(It.IsAny<Product>()));
        _mockProductRepository.Setup(p => p.Commit(default)).ReturnsAsync(1);

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.IsError.Should().BeFalse();
        result.Value.Should().BeOfType<Success>();

        _mockProductRepository.Verify(x => x.Remove(product), Times.Once);
        _mockProductRepository.Verify(x => x.Commit(CancellationToken.None), Times.Once);
    }

    // [Fact]
    // public async Task ShouldReturnErrorWhenTryingToDeleteANonExistentProduct()
    // {
    //     // Arrange
    //     var productId = ProductId.Create(99999); // Assuming this product ID doesn't exist in the database
    //     var command = new DeleteProductCommand { ProductId = productId.Value };

    //     _mockProductRepository
    //         .Setup(x => x.GetProductByIdAsync(productId, CancellationToken.None))
    //         .ReturnsAsync((Domain.Products.Product)null);

    //     // Act
    //     var result = await _sut.Handle(command, CancellationToken.None);

    //     // Assert
    //     Assert.False(result.IsSuccess);
    //     Assert.Equal(CustomErrors.Product.ProductNotFound, result.Errors.First());
    //     _mockProductRepository.Verify(
    //         x => x.Remove(It.IsAny<Domain.Products.Product>()),
    //         Times.Never
    //     );
    //     _mockProductRepository.Verify(x => x.Commit(CancellationToken.None), Times.Never);
    // }

    // [Fact]
    // public async Task ShouldHandleConcurrentDeleteRequestsForTheSameProduct()
    // {
    //     // Arrange
    //     var productId = ProductId.Create(1);
    //     var command = new DeleteProductCommand { ProductId = productId.Value };
    //     var product = new Domain.Products.Product(
    //         productId,
    //         "Test Product",
    //         "Test Description",
    //         100m
    //     );

    //     _mockProductRepository
    //         .Setup(x => x.GetProductByIdAsync(productId, CancellationToken.None))
    //         .ReturnsAsync(product);

    //     _mockProductRepository.Setup(x => x.Commit(CancellationToken.None)).ReturnsAsync(1);

    //     var task1 = _sut.Handle(command, CancellationToken.None);
    //     var task2 = _sut.Handle(command, CancellationToken.None);

    //     // Act
    //     await Task.WhenAll(task1, task2);

    //     // Assert
    //     Assert.True(task1.Result.IsSuccess);
    //     Assert.True(task2.Result.IsSuccess);
    //     Assert.Equal("Delete product successfully.", task1.Result.Data.Message);
    //     Assert.Equal("Delete product successfully.", task2.Result.Data.Message);
    //     _mockProductRepository.Verify(x => x.Remove(product), Times.Exactly(2));
    //     _mockProductRepository.Verify(x => x.Commit(CancellationToken.None), Times.Exactly(2));
    // }

    // [Fact]
    // public async Task ShouldRollbackChangesIfProductDeletionFails()
    // {
    //     // Arrange
    //     var productId = ProductId.Create(1);
    //     var command = new DeleteProductCommand { ProductId = productId.Value };
    //     var product = new Domain.Products.Product(
    //         productId,
    //         "Test Product",
    //         "Test Description",
    //         100m
    //     );

    //     _mockProductRepository
    //         .Setup(x => x.GetProductByIdAsync(productId, CancellationToken.None))
    //         .ReturnsAsync(product);

    //     _mockProductRepository.Setup(x => x.Commit(CancellationToken.None)).ReturnsAsync(0); // Simulate failure to commit changes

    //     // Act
    //     var result = await _sut.Handle(command, CancellationToken.None);

    //     // Assert
    //     Assert.False(result.IsSuccess);
    //     Assert.Equal("Product not found.", result.Data.Message);
    //     _mockProductRepository.Verify(x => x.Remove(product), Times.Once);
    //     _mockProductRepository.Verify(x => x.Commit(CancellationToken.None), Times.Once);
    //     _mockProductRepository.Verify(x => x.Rollback(), Times.Once);
    // }

    // [Fact]
    // public async Task ShouldDeleteProductImageFromMediaStorage()
    // {
    //     // Arrange
    //     var productId = ProductId.Create(1);
    //     var command = new DeleteProductCommand { ProductId = productId.Value };
    //     var product = new Domain.Products.Product(
    //         productId,
    //         "Test Product",
    //         "Test Description",
    //         100m
    //     );

    //     // Act
    //     await _sut.Handle(command, CancellationToken.None);

    //     // Assert
    //     _mediaUploaderMock.Verify(
    //         x => x.DeleteImageAsync(product.ImagePath, CancellationToken.None),
    //         Times.Once
    //     );
    // }

    // [Fact]
    // public async Task ShouldNotDeleteProductImageIfMediaDeletionFails()
    // {
    //     // Arrange
    //     var productId = ProductId.Create(1);
    //     var command = new DeleteProductCommand { ProductId = productId.Value };
    //     var product = new Domain.Products.Product(
    //         productId,
    //         "Test Product",
    //         "Test Description",
    //         100m
    //     );

    //     _mockProductRepository
    //         .Setup(x => x.GetProductByIdAsync(productId, CancellationToken.None))
    //         .ReturnsAsync(product);

    //     _mediaUploaderMock
    //         .Setup(x => x.DeleteMediaAsync(product.Image, CancellationToken.None))
    //         .ReturnsAsync(false);

    //     _mockProductRepository.Setup(x => x.Commit(CancellationToken.None)).ReturnsAsync(1);

    //     // Act
    //     var result = await _sut.Handle(command, CancellationToken.None);

    //     // Assert
    //     Assert.True(result.IsSuccess);
    //     Assert.Equal("Delete product successfully.", result.Data.Message);
    //     _mockProductRepository.Verify(x => x.Remove(product), Times.Once);
    //     _mockProductRepository.Verify(x => x.Commit(CancellationToken.None), Times.Once);
    //     _mediaUploaderMock.Verify(
    //         x => x.DeleteMediaAsync(product.Image, CancellationToken.None),
    //         Times.Once
    //     );
    // }

    // [Fact]
    // public async Task ShouldReturnSuccessResponseWithAppropriateMessage()
    // {
    //     // Arrange
    //     var productId = ProductId.Create(1);
    //     var command = new DeleteProductCommand { ProductId = productId.Value };
    //     var product = new Domain.Products.Product(
    //         productId,
    //         "Test Product",
    //         "Test Description",
    //         100m
    //     );

    //     _mockProductRepository
    //         .Setup(x => x.GetProductByIdAsync(productId, CancellationToken.None))
    //         .ReturnsAsync(product);

    //     _mockProductRepository.Setup(x => x.Commit(CancellationToken.None)).ReturnsAsync(1);

    //     // Act
    //     var result = await _sut.Handle(command, CancellationToken.None);

    //     // Assert
    //     Assert.True(result.IsSuccess);
    //     Assert.Equal("Delete product successfully.", result.Data.Message);
    //     _mockProductRepository.Verify(x => x.Remove(product), Times.Once);
    //     _mockProductRepository.Verify(x => x.Commit(CancellationToken.None), Times.Once);
    // }

    // [Fact]
    // public async Task ShouldReturnErrorResponseWhenProductIsNotFound()
    // {
    //     // Arrange
    //     var productId = ProductId.Create(9999); // Assuming this product does not exist
    //     var command = new DeleteProductCommand { ProductId = productId.Value };
    //     var productRepositoryMock = new Mock<IProductRepository>();

    //     productRepositoryMock
    //         .Setup(x => x.GetProductByIdAsync(productId, CancellationToken.None))
    //         .ReturnsAsync((Domain.Products.Product)null);

    //     var sut = new DeleteProductCommandHandler(
    //         productRepositoryMock.Object,
    //         new Mock<IMediaUploader>().Object
    //     );

    //     // Act
    //     var result = await sut.Handle(command, CancellationToken.None);

    //     // Assert
    //     Assert.False(result.IsSuccess);
    //     Assert.Equal(CustomErrors.Product.ProductNotFound.Message, result.Errors.First().Message);
    // }

    // [Fact]
    // public void ShouldValidateInputParametersForNullOrEmptyValues()
    // {
    //     // Arrange
    //     var productRepositoryMock = new Mock<IProductRepository>();
    //     var mediaUploaderMock = new Mock<IMediaUploader>();
    //     var sut = new DeleteProductCommandHandler(
    //         productRepositoryMock.Object,
    //         mediaUploaderMock.Object
    //     );

    //     // Act & Assert
    //     Assert.Throws<ArgumentNullException>(() => sut.Handle(null, CancellationToken.None));
    //     Assert.Throws<ArgumentException>(
    //         () => sut.Handle(new DeleteProductCommand(), CancellationToken.None)
    //     );
    // }

    // [Fact]
    // public async Task ShouldHandleDatabaseConnectionFailuresDuringProductDeletion()
    // {
    //     // Arrange
    //     var productId = ProductId.Create(1);
    //     var command = new DeleteProductCommand { ProductId = productId.Value };
    //     var product = new Domain.Products.Product(
    //         productId,
    //         "Test Product",
    //         "Test Description",
    //         100m
    //     );

    //     _mockProductRepository
    //         .Setup(x => x.GetProductByIdAsync(productId, CancellationToken.None))
    //         .ReturnsAsync(product);

    //     _mockProductRepository
    //         .Setup(x => x.Commit(CancellationToken.None))
    //         .ThrowsAsync(new DbException("Database connection failed"));

    //     // Act
    //     var result = await _sut.Handle(command, CancellationToken.None);

    //     // Assert
    //     Assert.False(result.IsSuccess);
    //     Assert.Equal(CustomErrors.Product.ProductNotFound, result.Errors.First());
    //     _mockProductRepository.Verify(x => x.Remove(product), Times.Never);
    //     _mockProductRepository.Verify(x => x.Commit(CancellationToken.None), Times.Once);
    // }
}
