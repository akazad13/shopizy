using Moq;
using Shopizy.Application.Carts.Commands.UpdateProductQuantity;
using Shopizy.Application.Common.Interfaces.Persistence;
using Shopizy.Application.UnitTests.Carts.TestUtils;
using Shopizy.Domain.Carts;
using Shopizy.Domain.Carts.ValueObjects;
using Shopizy.Domain.Common.CustomErrors;

namespace Shopizy.Application.UnitTests.Carts.Commands.UpdateProductQuantity;

public class UpdateProductQuantityCommandHandlerTests
{
    private readonly Mock<ICartRepository> _mockCartRepository;
    private readonly UpdateProductQuantityCommandHandler _sut;

    public UpdateProductQuantityCommandHandlerTests()
    {
        _mockCartRepository = new Mock<ICartRepository>();
        _sut = new UpdateProductQuantityCommandHandler(_mockCartRepository.Object);
    }

    // Should return cart not found when the cart is not found
    [Fact]
    public async Task Should_ReturnsCartNotFound_WhenCartIsNoFound()
    {
        // Arrange
        var command = UpdateProductQuantityCommandUtils.CreateCommand(37);

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
    }
}
