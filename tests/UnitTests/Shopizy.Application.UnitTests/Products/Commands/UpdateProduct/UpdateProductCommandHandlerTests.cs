using FluentAssertions;
using Moq;
using Shopizy.Application.Common.Interfaces.Persistence;
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
    public async Task UpdateProduct_WhenProductIsFound_CreateAndReturnProduct()
    {
        // Arrange
        UpdateProductCommand command = UpdateProductCommandUtils.CreateCommand();
        Domain.Products.Product product = ProductFactory.CreateProduct();

        _ = _mockProductRepository
            .Setup(p => p.GetProductByIdAsync(ProductId.Create(command.ProductId)))
            .ReturnsAsync(product);

        _ = _mockProductRepository.Setup(p => p.Update(product));
        _ = _mockProductRepository.Setup(p => p.Commit(default)).ReturnsAsync(1);
        // Act
        ErrorOr.ErrorOr<Domain.Products.Product> result = await _handler.Handle(command, default);

        // Assert
        _ = result.IsError.Should().BeFalse();
        result.Value.ValidateResult(command);

        _mockProductRepository.Verify(m => m.Update(result.Value), Times.Once);
        _mockProductRepository.Verify(m => m.Commit(default), Times.Once);
    }
}
