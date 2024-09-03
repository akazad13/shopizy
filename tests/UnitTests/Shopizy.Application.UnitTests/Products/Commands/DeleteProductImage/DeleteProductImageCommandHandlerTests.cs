using ErrorOr;
using FluentAssertions;
using Moq;
using Shopizy.Application.Common.Interfaces.Persistence;
using Shopizy.Application.Common.Interfaces.Services;
using Shopizy.Application.Products.Commands.DeleteProduct;
using Shopizy.Application.UnitTests.Products.TestUtils;
using Shopizy.Domain.Products.ValueObjects;

namespace Shopizy.Application.UnitTests.Products.Commands.DeleteProductImage;

public class DeleteProductImageCommandHandlerTests
{
    private readonly DeleteProductImageCommandHandler _handler;
    private readonly Mock<IProductRepository> _mockProductRepository;
    private readonly Mock<IMediaUploader> _mockMediaUploader;

    public DeleteProductImageCommandHandlerTests()
    {
        _mockProductRepository = new Mock<IProductRepository>();
        _mockMediaUploader = new Mock<IMediaUploader>();
        _handler = new DeleteProductImageCommandHandler(
            _mockProductRepository.Object,
            _mockMediaUploader.Object
        );
    }

    [Fact]
    public async Task DeleteProductImage_WhenProductIsFoundAndImageIsFound_DeleteAndReturnSuccess()
    {
        // Arrange
        Domain.Products.Product product = ProductFactory.CreateProduct();
        Domain.Products.Entities.ProductImage productImage = ProductFactory.CreateProductImage();
        product.AddProductImage(productImage);

        Application.Products.Commands.DeleteProductImage.DeleteProductImageCommand command = DeleteProductImageCommandUtils.CreateCommand(
            product.Id.Value,
            productImage.Id.Value
        );

        _ = _mockProductRepository
            .Setup(p => p.GetProductByIdAsync(ProductId.Create(command.ProductId)))
            .ReturnsAsync(product);

        _ = _mockMediaUploader
            .Setup(cl => cl.DeletePhotoAsync(productImage.PublicId))
            .ReturnsAsync(Result.Success);

        _ = _mockProductRepository.Setup(p => p.Update(product));
        _ = _mockProductRepository.Setup(p => p.Commit(default)).ReturnsAsync(1);
        // Act
        ErrorOr<Success> result = await _handler.Handle(command, default);

        // Assert
        _ = result.IsError.Should().BeFalse();
        _ = result.Value.Should().BeOfType(typeof(Success));

        _mockProductRepository.Verify(m => m.Commit(default), Times.Once);
    }
}
