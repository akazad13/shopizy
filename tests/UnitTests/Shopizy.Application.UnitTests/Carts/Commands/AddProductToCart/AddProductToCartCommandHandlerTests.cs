using FluentAssertions;
using Moq;
using Shopizy.Application.Carts.Commands.AddProductToCart;
using Shopizy.Application.Common.Interfaces.Persistance;
using Shopizy.Application.UnitTests.Carts.TestUtils;
using Shopizy.Domain.Carts;
using Shopizy.Domain.Carts.ValueObjects;
using Shopizy.Domain.Common.CustomErrors;
using Shopizy.Domain.Products.ValueObjects;

namespace Shopizy.Application.UnitTests.Carts.Commands.AddProductToCart;

public class AddProductToCartCommandHandlerTests
{
    private readonly AddProductToCartCommandHandler _handler;
    private readonly Mock<IProductRepository> _mockProductRepository;
    private readonly Mock<ICartRepository> _mockCartRepository;

    public AddProductToCartCommandHandlerTests()
    {
        _mockProductRepository = new Mock<IProductRepository>();
        _mockCartRepository = new Mock<ICartRepository>();
        _handler = new AddProductToCartCommandHandler(
            _mockCartRepository.Object,
            _mockProductRepository.Object
        );
    }

    [Fact]
    public async Task AddProductToCart_CartDoesNotExist_ReturnsCartNotFound()
    {
        // Arrange
        var cart = CartFactory.Create();
        var command = AddProductToCartCommandUtils.CreateCommand();
        _mockCartRepository
            .Setup(x => x.GetCartByIdAsync(CartId.Create(command.CartId)))
            .ReturnsAsync(() => null);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        _mockCartRepository.Verify(
            x => x.GetCartByIdAsync(CartId.Create(command.CartId)),
            Times.Once
        );
        _mockProductRepository.Verify(
            x => x.IsProductExistAsync(ProductId.Create(command.ProductId)),
            Times.Never
        );

        result.IsError.Should().BeTrue();
        result.Errors[0].Should().Be(CustomErrors.Cart.CartNotFound);

        _mockCartRepository.Verify(x => x.Update(cart), Times.Never);
        _mockCartRepository.Verify(x => x.Commit(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task AddProductToCart_GivenExistingCartAndValidProductId_ShouldAddProductAndReturnCartWithCorrectLineItem()
    {
        // Arrange
        var cart = CartFactory.Create();
        var command = AddProductToCartCommandUtils.CreateCommand();

        _mockCartRepository
            .Setup(cr => cr.GetCartByIdAsync(CartId.Create(command.CartId)))
            .ReturnsAsync(cart);

        _mockProductRepository
            .Setup(pr => pr.IsProductExistAsync(ProductId.Create(command.ProductId)))
            .ReturnsAsync(true);

        _mockCartRepository.Setup(cr => cr.Update(cart));
        _mockCartRepository.Setup(cr => cr.Commit(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsError.Should().BeFalse();
        result.Value.Should().BeOfType<Cart>();
        result.Value.Should().Be(cart);
        result.Value.LineItems.Should().HaveCount(1);
        result.Value.LineItems[0].ProductId.Should().BeOfType(typeof(ProductId));
        result.Value.LineItems[0].Quantity.Should().Be(1);

        _mockCartRepository.Verify(
            cr => cr.GetCartByIdAsync(CartId.Create(command.CartId)),
            Times.Once
        );
        _mockProductRepository.Verify(
            x => x.IsProductExistAsync(ProductId.Create(command.ProductId)),
            Times.Once
        );
        _mockCartRepository.Verify(cr => cr.Update(cart), Times.Once);
        _mockCartRepository.Verify(uow => uow.Commit(It.IsAny<CancellationToken>()), Times.Once);
    }
}
