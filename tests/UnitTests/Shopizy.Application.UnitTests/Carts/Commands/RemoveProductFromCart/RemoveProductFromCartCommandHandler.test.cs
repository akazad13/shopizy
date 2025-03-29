using ErrorOr;
using Moq;
using Shopizy.Application.Carts.Commands.RemoveProductFromCart;
using Shopizy.Application.Common.Interfaces.Persistence;
using Shopizy.Application.UnitTests.Carts.TestUtils;
using Shopizy.Domain.Carts;
using Shopizy.Domain.Carts.ValueObjects;
using Shopizy.Domain.Common.CustomErrors;

namespace Shopizy.Application.UnitTests.Carts.Commands.RemoveProductFromCart;

public class RemoveProductFromCartCommandHandlerTests
{
    private readonly Mock<ICartRepository> _mockCartRepository;
    private readonly RemoveProductFromCartCommandHandler _sut;

    public RemoveProductFromCartCommandHandlerTests()
    {
        _mockCartRepository = new Mock<ICartRepository>();
        _sut = new RemoveProductFromCartCommandHandler(_mockCartRepository.Object);
    }

    // Should return cart not found when cart is not found
    [Fact]
    public async Task Should_ReturnsCartNotFound_WhenCartIsNotFound()
    {
        // Arrange
        var command = RemoveProductFromCartCommandUtils.CreateCommand();

        _mockCartRepository
            .Setup(cr => cr.GetCartByIdAsync(CartId.Create(command.CartId), CancellationToken.None))
            .ReturnsAsync(() => null);

        // Act
        var result = await _sut.Handle(command, default);

        // Assert
        Assert.True(result.IsError);
        Assert.NotNull(result.Errors);
        Assert.Equal(CustomErrors.Cart.CartNotFound, result.Errors[0]);

        _mockCartRepository.Verify(
            cr => cr.GetCartByIdAsync(CartId.Create(command.CartId), CancellationToken.None),
            Times.Once
        );
        _mockCartRepository.Verify(x => x.Update(It.IsAny<Cart>()), Times.Never);
        _mockCartRepository.Verify(x => x.Commit(It.IsAny<CancellationToken>()), Times.Never);
    }

    // Should remove last line item from cart and commit changes when cart is found
    [Fact]
    public async Task Should_RemoveLastLineItemFromCart_WhenCartIsFound()
    {
        // Arrange
        var existingCart = CartFactory.Create();
        var command = RemoveProductFromCartCommandUtils.CreateCommand();

        _mockCartRepository
            .Setup(cr => cr.GetCartByIdAsync(CartId.Create(command.CartId), CancellationToken.None))
            .ReturnsAsync(() => existingCart);

        _mockCartRepository.Setup(cr => cr.Update(existingCart));
        _mockCartRepository.Setup(cr => cr.Commit(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(result.IsError);
        Assert.IsType<Success>(result.Value);

        _mockCartRepository.Verify(
            cr => cr.GetCartByIdAsync(CartId.Create(command.CartId), CancellationToken.None),
            Times.Once
        );
        _mockCartRepository.Verify(
            x => x.Update(It.Is<Cart>(c => c.CartItems.Count == 0)),
            Times.Once
        );
        _mockCartRepository.Verify(x => x.Commit(It.IsAny<CancellationToken>()), Times.Once);
    }
}
