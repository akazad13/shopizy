using ErrorOr;
using FluentAssertions;
using Moq;
using Shopizy.Application.Carts.Commands.RemoveProductsFromCart;
using Shopizy.Application.Common.Interfaces.Persistence;
using Shopizy.Application.UnitTests.Carts.TestUtils;
using Shopizy.Domain.Carts.ValueObjects;
using Shopizy.Domain.Common.CustomErrors;

namespace Shopizy.Application.UnitTests.Carts.Commands.RemoveProductsFromCart;

public class RemoveProductsFromCartCommandHandlerTests
{
    private readonly RemoveProductFromCartCommandHandler _handler;
    private readonly Mock<ICartRepository> _mockCartRepository;

    public RemoveProductsFromCartCommandHandlerTests()
    {
        _mockCartRepository = new Mock<ICartRepository>();
        _handler = new RemoveProductFromCartCommandHandler(_mockCartRepository.Object);
    }

    [Fact]
    public async Task RemoveProductsFromCart_WhenCartIsNoFound_ReturnsCartNotFound()
    {
        // Arrange

        var command = RemoveProductsFromCartCommandUtils.CreateCommand();
        var cart = CartFactory.Create();

        _mockCartRepository
            .Setup(cr => cr.GetCartByIdAsync(CartId.Create(command.CartId)))
            .ReturnsAsync(() => null);

        // Act
        var result = await _handler.Handle(command, default);

        // Assert
        result.IsError.Should().BeTrue();
        result.Errors[0].Should().Be(CustomErrors.Cart.CartNotFound);

        _mockCartRepository.Verify(
            cr => cr.GetCartByIdAsync(CartId.Create(command.CartId)),
            Times.Once
        );
        _mockCartRepository.Verify(x => x.Update(cart), Times.Never);
        _mockCartRepository.Verify(x => x.Commit(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task RemoveProductsFromCart_WhenCartIsFound_RemoveProductsAndReturnSuccess()
    {
        // Arrange

        var command = RemoveProductsFromCartCommandUtils.CreateCommand();
        var cart = CartFactory.Create();

        _mockCartRepository
            .Setup(cr => cr.GetCartByIdAsync(CartId.Create(command.CartId)))
            .ReturnsAsync(cart);

        _mockCartRepository.Setup(cr => cr.Update(cart));
        _mockCartRepository.Setup(cr => cr.Commit(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        // Act
        var result = await _handler.Handle(command, default);

        // Assert
        result.IsError.Should().BeFalse();
        result.Value.Should().BeOfType<Success>();

        _mockCartRepository.Verify(
            cr => cr.GetCartByIdAsync(CartId.Create(command.CartId)),
            Times.Once
        );
        _mockCartRepository.Verify(cr => cr.Update(cart), Times.Once);
        _mockCartRepository.Verify(uow => uow.Commit(It.IsAny<CancellationToken>()), Times.Once);
    }
}
