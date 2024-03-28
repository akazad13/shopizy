using FluentAssertions;
using Moq;
using Shopizy.Application.Common.Interfaces.Persistance;
using Shopizy.Application.Common.Interfaces.Services;
using Shopizy.Application.Products.Commands.DeleteProduct;
using Shopizy.Application.UnitTests.Products.Commands.TestUtils;
using Shopizy.Application.UnitTests.TestUtils.Constants;
using Shopizy.Domain.Products.ValueObjects;

namespace Shopizy.Application.UnitTests.Products.Commands.DeleteProduct;

public class DeleteProductCommandHandlerTests
{
    private readonly DeleteProductCommandHandler _handler;
    private readonly Mock<IProductRepository> _mockProductRepository;
    private readonly Mock<IMediaUploader> _mockMediaUploader;

    public DeleteProductCommandHandlerTests()
    {
        _mockProductRepository = new Mock<IProductRepository>();
        _mockMediaUploader = new Mock<IMediaUploader>();
        _handler = new DeleteProductCommandHandler(
            _mockProductRepository.Object,
            _mockMediaUploader.Object
        );
    }

    [Fact]
    public async void DeleteProduct_WhenProductIsFound_ShouldDeleteAndReturnSuccess()
    {
        // Arrange

        var deleteProductCmd = DeleteProductCommandUtils.CreateCommand();
        var product = Constants.Product.NewProduct;

        _mockProductRepository
            .Setup(p => p.GetProductByIdAsync(ProductId.Create(deleteProductCmd.ProductId)))
            .ReturnsAsync(product);

        _mockProductRepository.Setup(p => p.Remove(product));
        _mockProductRepository.Setup(p => p.Commit(default)).ReturnsAsync(1);
        // Act
        var result = await _handler.Handle(deleteProductCmd, default);

        // Assert
        result.IsError.Should().BeFalse();

        _mockProductRepository.Verify(m => m.Commit(default), Times.Once);
    }
}
