using FluentAssertions;
using Moq;
using Shopizy.Application.Common.Interfaces.Persistence;
using Shopizy.Application.Common.Interfaces.Services;
using Shopizy.Application.Products.Commands.DeleteProduct;
using Shopizy.Application.UnitTests.Products.TestUtils;
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
    public async Task DeleteProduct_WhenProductIsFound_DeleteAndReturnSuccess()
    {
        // Arrange

        DeleteProductCommand command = DeleteProductCommandUtils.CreateCommand();
        Domain.Products.Product product = ProductFactory.CreateProduct();

        _ = _mockProductRepository
            .Setup(p => p.GetProductByIdAsync(ProductId.Create(command.ProductId)))
            .ReturnsAsync(product);

        _ = _mockProductRepository.Setup(p => p.Remove(product));
        _ = _mockProductRepository.Setup(p => p.Commit(default)).ReturnsAsync(1);
        // Act
        ErrorOr.ErrorOr<ErrorOr.Success> result = await _handler.Handle(command, default);

        // Assert
        _ = result.IsError.Should().BeFalse();

        _mockProductRepository.Verify(m => m.Commit(default), Times.Once);
    }
}
