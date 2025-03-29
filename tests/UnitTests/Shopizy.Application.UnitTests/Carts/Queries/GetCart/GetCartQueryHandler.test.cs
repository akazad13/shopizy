using Moq;
using Shopizy.Application.Carts.Queries.GetCart;
using Shopizy.Application.Common.Interfaces.Persistence;
using Shopizy.Application.UnitTests.Carts.TestUtils;
using Shopizy.Application.UnitTests.TestUtils.Extensions;
using Shopizy.Domain.Carts;
using Shopizy.Domain.Users.ValueObjects;

namespace Shopizy.Application.UnitTests.Carts.Queries.GetCart;

public class GetCartQueryHandlerTests
{
    private readonly GetCartQueryHandler _sut;
    private readonly Mock<ICartRepository> _mockCartRepository;

    public GetCartQueryHandlerTests()
    {
        _mockCartRepository = new Mock<ICartRepository>();
        _sut = new GetCartQueryHandler(_mockCartRepository.Object);
    }

    [Fact]
    public async Task Should_ReturnValidCart_WhenUserHasItemsInCart()
    {
        // Arrange
        var cart = CartFactory.Create();
        cart.AddLineItem(CartFactory.CreateCartItem());
        var query = GetCartQueryUtils.CreateQuery();

        _mockCartRepository
            .Setup(repo => repo.GetCartByUserIdAsync(It.IsAny<UserId>()))
            .ReturnsAsync(cart);

        // Act
        var result = await _sut.Handle(query, CancellationToken.None);

        // Assert
        Assert.False(result.IsError);
        Assert.NotNull(result.Value);
        Assert.IsType<Cart>(result.Value);
        result.Value.ValidateResult(query);
    }
}
