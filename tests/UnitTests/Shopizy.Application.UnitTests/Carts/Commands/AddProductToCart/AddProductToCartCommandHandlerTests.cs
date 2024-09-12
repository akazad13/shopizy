using FluentAssertions;
using Moq;
using Shopizy.Application.Carts.Commands.AddProductToCart;
using Shopizy.Application.Common.Interfaces.Persistence;
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
        Cart cart = CartFactory.Create();
        AddProductToCartCommand command = AddProductToCartCommandUtils.CreateCommand();
        _ = _mockCartRepository
            .Setup(x => x.GetCartByIdAsync(CartId.Create(command.CartId)))
            .ReturnsAsync(() => null);

        // Act
        ErrorOr.ErrorOr<Cart> result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        _mockCartRepository.Verify(
            x => x.GetCartByIdAsync(CartId.Create(command.CartId)),
            Times.Once
        );
        _mockProductRepository.Verify(
            x => x.IsProductExistAsync(ProductId.Create(command.ProductId)),
            Times.Never
        );

        _ = result.IsError.Should().BeTrue();
        _ = result.Errors[0].Should().Be(CustomErrors.Cart.CartNotFound);

        _mockCartRepository.Verify(x => x.Update(cart), Times.Never);
        _mockCartRepository.Verify(x => x.Commit(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task AddProductToCart_GivenExistingCartAndValidProductId_AddProductAndReturnCart()
    {
        // Arrange
        Cart cart = CartFactory.Create();
        AddProductToCartCommand command = AddProductToCartCommandUtils.CreateCommand();

        _ = _mockCartRepository
            .Setup(cr => cr.GetCartByIdAsync(CartId.Create(command.CartId)))
            .ReturnsAsync(cart);

        _ = _mockProductRepository
            .Setup(pr => pr.IsProductExistAsync(ProductId.Create(command.ProductId)))
            .ReturnsAsync(true);

        _ = _mockCartRepository.Setup(cr => cr.Update(cart));
        _ = _mockCartRepository.Setup(cr => cr.Commit(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        _ = _mockCartRepository.Setup(cr => cr.GetCartByUserIdAsync(cart.UserId)).ReturnsAsync(cart);

        // Act
        ErrorOr.ErrorOr<Cart> result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        _ = result.IsError.Should().BeFalse();
        _ = result.Value.Should().BeOfType<Cart>();
        _ = result.Value.Should().Be(cart);
        _ = result.Value.LineItems.Should().HaveCount(1);
        _ = result.Value.LineItems[0].ProductId.Should().BeOfType(typeof(ProductId));
        _ = result.Value.LineItems[0].Quantity.Should().Be(1);

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
        _mockCartRepository.Verify(cr => cr.GetCartByUserIdAsync(cart.UserId), Times.Once);
    }
}
