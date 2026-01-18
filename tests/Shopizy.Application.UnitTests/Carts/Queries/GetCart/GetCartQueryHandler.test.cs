using Moq;
using Shouldly;
using Shopizy.Application.Carts.Queries.GetCart;
using Shopizy.Application.Common.Interfaces.Persistence;
using Shopizy.Domain.Carts;
using Shopizy.Domain.Users.ValueObjects;

namespace Shopizy.Application.UnitTests.Carts.Queries.GetCart;

public class GetCartQueryHandlerTestsRefactored
{
    private readonly Mock<ICartRepository> _mockCartRepository;
    private readonly GetCartQueryHandler _handler;

    public GetCartQueryHandlerTestsRefactored()
    {
        _mockCartRepository = new Mock<ICartRepository>();
        _handler = new GetCartQueryHandler(_mockCartRepository.Object);
    }

    [Fact]
    public async Task Handle_WhenCartExists_ShouldReturnCart()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var cart = Cart.Create(UserId.Create(userId));
        var query = new GetCartQuery(userId);

        _mockCartRepository.Setup(r => r.GetCartByUserIdAsync(It.IsAny<UserId>()))
            .ReturnsAsync(cart);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsError.ShouldBeFalse();
        result.Value.ShouldBe(cart);
    }

    [Fact]
    public async Task Handle_WhenCartDoesNotExist_ShouldReturnError()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var query = new GetCartQuery(userId);

        _mockCartRepository.Setup(r => r.GetCartByUserIdAsync(It.IsAny<UserId>()))
            .ReturnsAsync((Cart?)null);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsError.ShouldBeTrue();
    }
}
