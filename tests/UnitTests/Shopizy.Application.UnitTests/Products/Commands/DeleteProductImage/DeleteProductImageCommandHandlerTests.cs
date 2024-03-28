using FluentAssertions;
using Moq;
using Shopizy.Application.Common.Interfaces.Persistance;
using Shopizy.Application.Common.Interfaces.Services;
using Shopizy.Application.UnitTests.Products.Commands.TestUtils;
using Shopizy.Application.UnitTests.TestUtils.Constants;
using Shopizy.Domain.Products.ValueObjects;
using Shopizy.Domain.Products.Entities;
using Shopizy.Application.Products.Commands.DeleteProduct;
using ErrorOr;

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
    public async void DeleteProductImage_WhenProductIsFoundAndImageIsFound_ShouldDeleteAndReturnSuccess()
    {
        // Arrange
        var product = Constants.Product.NewProduct;
        var productImage = Constants.ProductImage.NewProductImage;
        product.AddProductImage(productImage);
        var deleteProductCmd = DeleteProductImageCommandUtils.CreateCommand(
            product.Id.Value,
            productImage.Id.Value
        );

        _mockProductRepository
            .Setup(p => p.GetProductByIdAsync(ProductId.Create(deleteProductCmd.ProductId)))
            .ReturnsAsync(product);

        _mockMediaUploader
            .Setup(cl => cl.DeletePhotoAsync(productImage.PublicId))
            .ReturnsAsync(Result.Success);

        _mockProductRepository.Setup(p => p.Update(product));
        _mockProductRepository.Setup(p => p.Commit(default)).ReturnsAsync(1);
        // Act
        var result = await _handler.Handle(deleteProductCmd, default);

        // Assert
        result.IsError.Should().BeFalse();
        result.Value.Should().BeOfType(typeof(Success));

        _mockProductRepository.Verify(m => m.Commit(default), Times.Once);
    }
}
