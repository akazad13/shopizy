using ErrorOr;
using Moq;
using Shopizy.Application.Common.Interfaces.Persistence;
using Shopizy.Application.Common.Interfaces.Services;
using Shopizy.Application.Products.Commands.DeleteProduct;
using Shopizy.Application.UnitTests.Products.TestUtils;
using Shopizy.Domain.Common.CustomErrors;
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
        Assert.False(result.IsError);
        Assert.IsType<Success>(result.Value);

        _mockProductRepository.Verify(x => x.Remove(product), Times.Once);
        _mockProductRepository.Verify(x => x.Commit(CancellationToken.None), Times.Once);
    }

    [Fact]
    public async Task Should_ReturnProductNotFound_When_ProductDoesNotExist()
    {
        // Arrange
        var command = DeleteProductCommandUtils.CreateCommand();

        _mockProductRepository
            .Setup(p => p.GetProductByIdAsync(ProductId.Create(command.ProductId)))
            .ReturnsAsync((Product?)null);

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsError);
        Assert.Equal(CustomErrors.Product.ProductNotFound, result.FirstError);

        _mockProductRepository.Verify(x => x.GetProductByIdAsync(It.IsAny<ProductId>()), Times.Once);
        _mockProductRepository.Verify(x => x.Remove(It.IsAny<Product>()), Times.Never);
        _mockProductRepository.Verify(x => x.Commit(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Should_ReturnProductNotDeleted_When_RepositoryCommitFails()
    {
        // Arrange
        var command = DeleteProductCommandUtils.CreateCommand();
        var product = ProductFactory.CreateProduct();

        _mockProductRepository
            .Setup(p => p.GetProductByIdAsync(ProductId.Create(command.ProductId)))
            .ReturnsAsync(product);

        _mockProductRepository.Setup(p => p.Remove(It.IsAny<Product>()));
        _mockProductRepository.Setup(p => p.Commit(It.IsAny<CancellationToken>())).ReturnsAsync(0);

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsError);
        Assert.Equal(CustomErrors.Product.ProductNotDeleted, result.FirstError);

        _mockProductRepository.Verify(x => x.GetProductByIdAsync(It.IsAny<ProductId>()), Times.Once);
        _mockProductRepository.Verify(x => x.Remove(product), Times.Once);
        _mockProductRepository.Verify(x => x.Commit(It.IsAny<CancellationToken>()), Times.Once);
    }
}
