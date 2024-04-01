using FluentAssertions;
using Moq;
using Shopizy.Application.Common.Interfaces.Persistance;
using Shopizy.Application.Carts.Commands.CreateCartWithFirstProduct;
using Shopizy.Domain.Carts;
using Shopizy.Domain.Common.CustomErrors;
using Shopizy.Domain.Products.ValueObjects;
using Shopizy.Application.UnitTests.Carts.TestUtils;
using Shopizy.Domain.Users.ValueObjects;

namespace Shopizy.Application.UnitTests.Carts.Commands.CreateCartWithFirstProduct;

public class CreateCartWithFirstProductCommandHandlerTests
{
    private readonly CreateCartWithFirstProductCommandHandler _handler;
    private readonly Mock<IProductRepository> _mockProductRepository;
    private readonly Mock<ICartRepository> _mockCartRepository;

    public CreateCartWithFirstProductCommandHandlerTests()
    {
        _mockProductRepository = new Mock<IProductRepository>();
        _mockCartRepository = new Mock<ICartRepository>();
        _handler = new CreateCartWithFirstProductCommandHandler(
            _mockProductRepository.Object,
            _mockCartRepository.Object
        );
    }

    [Fact]
    public async Task CreateCartWithFirstProduct_ProductDoesNotExist_ReturnsProductNotFound()
    {
        // Arrange
        var command = CreateCartWithFirstProductCommandUtils.CreateCommand();
        _mockProductRepository
            .Setup(x => x.IsProductExistAsync(It.IsAny<ProductId>()))
            .ReturnsAsync(false);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        _mockProductRepository.Verify(
            x => x.IsProductExistAsync(ProductId.Create(command.ProductId)),
            Times.Once
        );
        _mockCartRepository.Verify(x => x.AddAsync(It.IsAny<Cart>()), Times.Never);
        _mockCartRepository.Verify(x => x.Commit(It.IsAny<CancellationToken>()), Times.Never);

        result.IsError.Should().BeTrue();
        result.Errors.First().Should().Be(CustomErrors.Product.ProductNotFound);
    }

    [Fact]
    public async Task CreateCartWithFirstProduct_ProductExists_CreatesCartWithLineItemAndReturnsCart()
    {
        // Arrange
        var command = CreateCartWithFirstProductCommandUtils.CreateCommand();
        _mockProductRepository
            .Setup(x => x.IsProductExistAsync(ProductId.Create(command.ProductId)))
            .ReturnsAsync(true);

        _mockCartRepository.Setup(x => x.AddAsync(It.IsAny<Cart>()));
        _mockCartRepository.Setup(x => x.Commit(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        _mockProductRepository.Verify(
            x => x.IsProductExistAsync(ProductId.Create(command.ProductId)),
            Times.Once
        );
        _mockCartRepository.Verify(
            x =>
                x.AddAsync(
                    It.Is<Cart>(c => c.UserId.Value == command.UserId && c.LineItems.Count == 1)
                ),
            Times.Once
        );
        _mockCartRepository.Verify(x => x.Commit(It.IsAny<CancellationToken>()), Times.Once);

        result.IsError.Should().BeFalse();
        result.Value.Should().BeOfType(typeof(Cart));
        result.Value.UserId.Should().Be(UserId.Create(command.UserId));
        result.Value.LineItems.Should().HaveCount(1);
        result.Value.LineItems[0].ProductId.Should().Be(ProductId.Create(command.ProductId));
    }
}
