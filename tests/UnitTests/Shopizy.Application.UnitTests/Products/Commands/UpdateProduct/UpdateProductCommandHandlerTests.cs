using FluentAssertions;
using Moq;
using Shopizy.Application.Common.Interfaces.Persistance;
using Shopizy.Application.Products.Commands.UpdateProduct;
using Shopizy.Application.UnitTests.Products.TestUtils;
using Shopizy.Application.UnitTests.TestUtils.Extensions;
using Shopizy.Domain.Products.ValueObjects;

namespace Shopizy.Application.UnitTests.Products.Commands.UpdateProduct;

public class UpdateProductCommandHandlerTests
{
    private readonly UpdateProductCommandHandler _handler;
    private readonly Mock<IProductRepository> _mockProductRepository;

    public UpdateProductCommandHandlerTests()
    {
        _mockProductRepository = new Mock<IProductRepository>();
        _handler = new UpdateProductCommandHandler(_mockProductRepository.Object);
    }

    [Fact]
    public async void UpdateProduct_WhenProductIsFound_ShouldCreateAndReturnProduct()
    {
        // Arrange
        var command = UpdateProductCommandUtils.CreateCommand();
        var product = ProductFactory.CreateProduct();

        _mockProductRepository
            .Setup(p => p.GetProductByIdAsync(ProductId.Create(command.ProductId)))
            .ReturnsAsync(product);

        _mockProductRepository.Setup(p => p.Update(product));
        _mockProductRepository.Setup(p => p.Commit(default)).ReturnsAsync(1);
        // Act
        var result = await _handler.Handle(command, default);

        // Assert
        result.IsError.Should().BeFalse();
        result.Value.ValidateResult(command);

        _mockProductRepository.Verify(m => m.Update(result.Value), Times.Once);
        _mockProductRepository.Verify(m => m.Commit(default), Times.Once);
    }
}
