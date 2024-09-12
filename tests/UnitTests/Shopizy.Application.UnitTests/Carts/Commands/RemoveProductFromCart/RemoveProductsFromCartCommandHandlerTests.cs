using ErrorOr;
using FluentAssertions;
using Moq;
using Shopizy.Application.Carts.Commands.RemoveProductFromCart;
using Shopizy.Application.Common.Interfaces.Persistence;
using Shopizy.Application.UnitTests.Carts.TestUtils;
using Shopizy.Domain.Carts.ValueObjects;
using Shopizy.Domain.Common.CustomErrors;

namespace Shopizy.Application.UnitTests.Carts.Commands.RemoveProductFromCart;

public class RemoveProductFromCartCommandHandlerTests
{
    private readonly RemoveProductFromCartCommandHandler _handler;
    private readonly Mock<ICartRepository> _mockCartRepository;

    public RemoveProductFromCartCommandHandlerTests()
    {
        _mockCartRepository = new Mock<ICartRepository>();
        _handler = new RemoveProductFromCartCommandHandler(_mockCartRepository.Object);
    }

    [Fact]
    public async Task RemoveProductFromCart_WhenCartIsNoFound_ReturnsCartNotFound()
    {
        // Arrange

        RemoveProductFromCartCommand command = RemoveProductFromCartCommandUtils.CreateCommand();
        Domain.Carts.Cart cart = CartFactory.Create();

        _ = _mockCartRepository
            .Setup(cr => cr.GetCartByIdAsync(CartId.Create(command.CartId)))
            .ReturnsAsync(() => null);

        // Act
        ErrorOr<Success> result = await _handler.Handle(command, default);

        // Assert
        _ = result.IsError.Should().BeTrue();
        _ = result.Errors[0].Should().Be(CustomErrors.Cart.CartNotFound);

        _mockCartRepository.Verify(
            cr => cr.GetCartByIdAsync(CartId.Create(command.CartId)),
            Times.Once
        );
        _mockCartRepository.Verify(x => x.Update(cart), Times.Never);
        _mockCartRepository.Verify(x => x.Commit(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task RemoveProductFromCart_WhenCartIsFound_RemoveProductAndReturnSuccess()
    {
        // Arrange

        RemoveProductFromCartCommand command = RemoveProductFromCartCommandUtils.CreateCommand();
        Domain.Carts.Cart cart = CartFactory.Create();

        _ = _mockCartRepository
            .Setup(cr => cr.GetCartByIdAsync(CartId.Create(command.CartId)))
            .ReturnsAsync(cart);

        _ = _mockCartRepository.Setup(cr => cr.Update(cart));
        _ = _mockCartRepository
            .Setup(cr => cr.Commit(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        // Act
        ErrorOr<Success> result = await _handler.Handle(command, default);

        // Assert
        _ = result.IsError.Should().BeFalse();
        _ = result.Value.Should().BeOfType<Success>();

        _mockCartRepository.Verify(
            cr => cr.GetCartByIdAsync(CartId.Create(command.CartId)),
            Times.Once
        );
        _mockCartRepository.Verify(cr => cr.Update(cart), Times.Once);
        _mockCartRepository.Verify(uow => uow.Commit(It.IsAny<CancellationToken>()), Times.Once);
    }
}
