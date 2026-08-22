using ErrorOr;
using Moq;
using Shopizy.Application.Common.Interfaces.Persistence;
using Shopizy.Application.Common.Interfaces.Services;
using Shopizy.Application.Products.Commands.DeleteProductImage;
using Shopizy.Application.UnitTests.Products.TestUtils;
using Shopizy.Domain.Products.ValueObjects;

namespace Shopizy.Application.UnitTests.Products.Commands.DeleteProductImage;

public class DeleteProductImageCommandHandlerTests
{
    private readonly Mock<IProductRepository> _mockProductRepository;
    private readonly Mock<IMediaUploader> _mockMediaUploader;
    private readonly DeleteProductImageCommandHandler _sut;

    public DeleteProductImageCommandHandlerTests()
    {
        _mockProductRepository = new Mock<IProductRepository>();
        _mockMediaUploader = new Mock<IMediaUploader>();
        _sut = new DeleteProductImageCommandHandler(
            _mockProductRepository.Object,
            _mockMediaUploader.Object
        );
    }

    [Fact]
    public async Task Should_DeleteAndReturnSuccess_WhenProductIsFoundAndImageIsFound()
    {
        // Arrange
        var product = ProductFactory.CreateProduct();
        var productImage = ProductFactory.CreateProductImage();
        product.AddProductImage(productImage);

        var command = DeleteProductImageCommandUtils.CreateCommand(
            Guid.NewGuid(),
            product.Id.Value,
            productImage.Id.Value
        );

        _mockProductRepository
            .Setup(p => p.GetProductByIdForUpdateAsync(ProductId.Create(command.ProductId)))
            .ReturnsAsync(product);

        _mockMediaUploader
            .Setup(cl => cl.DeletePhotoAsync(productImage.PublicId))
            .ReturnsAsync(Result.Success);

        _mockProductRepository.Setup(p => p.Update(product));
        // Act
        var result = await _sut.Handle(command, TestContext.Current.CancellationToken);

        // Assert
        Assert.False(result.IsError);
        Assert.IsType<Success>(result.Value);
    }

    [Fact]
    public async Task Should_ReturnError_WhenProductNotFound()
    {
        var command = DeleteProductImageCommandUtils.CreateCommand(
            Guid.NewGuid(),
            Guid.NewGuid(),
            Guid.NewGuid()
        );
        _mockProductRepository
            .Setup(p => p.GetProductByIdForUpdateAsync(It.IsAny<ProductId>()))
            .ReturnsAsync((Shopizy.Domain.Products.Product?)null);

        var result = await _sut.Handle(command, CancellationToken.None);

        Assert.True(result.IsError);
    }

    [Fact]
    public async Task Should_ReturnError_WhenImageNotFoundInProduct()
    {
        var product = ProductFactory.CreateProduct();
        var command = DeleteProductImageCommandUtils.CreateCommand(
            Guid.NewGuid(),
            product.Id.Value,
            Guid.NewGuid()
        );

        _mockProductRepository
            .Setup(p => p.GetProductByIdForUpdateAsync(It.IsAny<ProductId>()))
            .ReturnsAsync(product);

        var result = await _sut.Handle(command, CancellationToken.None);

        Assert.True(result.IsError);
    }

    [Fact]
    public async Task Should_ReturnError_WhenMediaUploaderFails()
    {
        var product = ProductFactory.CreateProduct();
        var productImage = ProductFactory.CreateProductImage();
        product.AddProductImage(productImage);

        var command = DeleteProductImageCommandUtils.CreateCommand(
            Guid.NewGuid(),
            product.Id.Value,
            productImage.Id.Value
        );

        _mockProductRepository
            .Setup(p => p.GetProductByIdForUpdateAsync(It.IsAny<ProductId>()))
            .ReturnsAsync(product);
        _mockMediaUploader
            .Setup(cl => cl.DeletePhotoAsync(productImage.PublicId))
            .ReturnsAsync(Error.Failure("Upload.Failed", "Failed to delete"));

        var result = await _sut.Handle(command, CancellationToken.None);

        Assert.True(result.IsError);
    }
}
