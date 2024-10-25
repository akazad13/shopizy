using FluentAssertions;
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
    public async Task ShouldReturnValidCartWhenUserHasItemsInCartAsync()
    {
        // Arrange
        var cart = CartFactory.Create();
        cart.AddLineItem(CartFactory.CreateLineItem());
        var query = GetCartQueryUtils.CreateQuery();

        _mockCartRepository
            .Setup(repo => repo.GetCartByUserIdAsync(It.IsAny<UserId>()))
            .ReturnsAsync(cart);

        // Act
        var result = (await _sut.Handle(query, CancellationToken.None)).Match(x => x, x => cart);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeOfType<Cart>();
        result.ValidateResult(query);
    }

    // [Fact]
    // public async Task ShouldReturnNull_WhenUserHasNoItemsInCart()
    // {
    //     // Arrange
    //     var userId = UserId.Create("test-user-id");
    //     var query = new GetCartQuery { UserId = userId.Value };
    //     _cartRepositoryMock
    //         .Setup(x => x.GetCartByUserIdAsync(userId, It.IsAny<CancellationToken>()))
    //         .ReturnsAsync((Cart?)null);

    //     var sut = new GetCartQueryHandler(_cartRepositoryMock.Object);

    //     // Act
    //     var result = await sut.Handle(query, CancellationToken.None);

    //     // Assert
    //     Assert.IsType<Response<Cart?>>(result);
    //     Assert.False(result.Success);
    //     Assert.Null(result.Data);
    // }

    // [Fact]
    // public async Task ShouldHandleConcurrentRequestsForTheSameUserWithoutDataCorruption()
    // {
    //     // Arrange
    //     var userId = "123";
    //     var cart = new Cart(UserId.Create(userId));
    //     _cartRepositoryMock
    //         .Setup(x => x.GetCartByUserIdAsync(It.IsAny<UserId>(), It.IsAny<CancellationToken>()))
    //         .ReturnsAsync(cart);

    //     var sut = new GetCartQueryHandler(_cartRepositoryMock.Object);

    //     // Act
    //     var task1 = sut.Handle(new GetCartQuery(userId), CancellationToken.None);
    //     var task2 = sut.Handle(new GetCartQuery(userId), CancellationToken.None);

    //     await Task.WhenAll(task1, task2);

    //     // Assert
    //     _cartRepositoryMock.Verify(
    //         x => x.GetCartByUserIdAsync(It.IsAny<UserId>(), It.IsAny<CancellationToken>()),
    //         Times.Once
    //     );
    //     Assert.Equal(task1.Result.Data, task2.Result.Data);
    // }

    // [Fact]
    // public async Task ShouldThrowExceptionWhenUserIdIsInvalid()
    // {
    //     // Arrange
    //     var query = new GetCartQuery { UserId = "invalid-user-id" };
    //     var handler = new GetCartQueryHandler(_cartRepositoryMock.Object);

    //     // Act & Assert
    //     await Should.ThrowAsync<ArgumentException>(
    //         async () => await handler.Handle(query, CancellationToken.None)
    //     );
    // }

    // [Fact]
    // public async Task ShouldReturnErrorResponseWhenDatabaseConnectionIsLost()
    // {
    //     // Arrange
    //     var mockCartRepository = new Mock<ICartRepository>();
    //     mockCartRepository
    //         .Setup(x => x.GetCartByUserIdAsync(It.IsAny<UserId>(), It.IsAny<CancellationToken>()))
    //         .ThrowsAsync(new System.Data.SqlClient.SqlException());

    //     var queryHandler = new GetCartQueryHandler(mockCartRepository.Object);
    //     var query = new GetCartQuery { UserId = "user123" };

    //     // Act
    //     var result = await queryHandler.Handle(query, CancellationToken.None);

    //     // Assert
    //     Assert.IsType<Result<Cart?>>(result);
    //     Assert.False(result.IsSuccess);
    //     Assert.Equal("An error occurred while processing your request.", result.ErrorMessage);
    // }

    // [Fact]
    // public async Task ShouldReturnEmptyCartWhenUserCartIsDeleted()
    // {
    //     // Arrange
    //     var userId = "user123";
    //     var query = new GetCartQuery { UserId = userId };
    //     _cartRepositoryMock
    //         .Setup(repo =>
    //             repo.GetCartByUserIdAsync(UserId.Create(userId), It.IsAny<CancellationToken>())
    //         )
    //         .ReturnsAsync((Cart?)null);

    //     var handler = new GetCartQueryHandler(_cartRepositoryMock.Object);

    //     // Act
    //     var result = await handler.Handle(query, CancellationToken.None);

    //     // Assert
    //     Assert.IsType<Response<Cart?>>(result);
    //     Assert.False(result.IsSuccess);
    //     Assert.Null(result.Data);
    // }

    // [Fact]
    // public async Task ShouldReturnCartWithCorrectTotalPrice_WhenItemsHaveDifferentPrices()
    // {
    //     // Arrange
    //     var userId = UserId.Create("test-user-id");
    //     var cart = new Cart(userId, 100); // Replace 100 with actual total price
    //     _cartRepository
    //         .Setup(x => x.GetCartByUserIdAsync(userId, It.IsAny<CancellationToken>()))
    //         .ReturnsAsync(cart);

    //     var queryHandler = new GetCartQueryHandler(_cartRepository.Object);
    //     var query = new GetCartQuery { UserId = userId.Value };

    //     // Act
    //     var result = await queryHandler.Handle(query, CancellationToken.None);

    //     // Assert
    //     Assert.True(result.IsSuccess);
    //     Assert.NotNull(result.Value);
    //     Assert.Equal(cart.TotalPrice, result.Value.TotalPrice);
    // }

    // [Fact]
    // public async Task ShouldReturnCartWithCorrectQuantityWhenItemsAddedMultipleTimes()
    // {
    //     // Arrange
    //     var mockCartRepository = new Mock<ICartRepository>();
    //     var userId = UserId.Create("testUser");
    //     var cart = new Cart(userId);
    //     cart.AddItem(new CartItem(1, "Product1", 10, 2));
    //     cart.AddItem(new CartItem(2, "Product2", 5, 3));

    //     mockCartRepository
    //         .Setup(repo => repo.GetCartByUserIdAsync(userId, It.IsAny<CancellationToken>()))
    //         .ReturnsAsync(cart);

    //     var sut = new GetCartQueryHandler(mockCartRepository.Object);
    //     var query = new GetCartQuery { UserId = userId.Value };

    //     // Act
    //     var result = await sut.Handle(query, CancellationToken.None);

    //     // Assert
    //     Assert.True(result.IsSuccess);
    //     Assert.NotNull(result.Value);
    //     Assert.Equal(2, result.Value.Items.Count);
    //     Assert.Equal(20, result.Value.Items[0].TotalPrice);
    //     Assert.Equal(15, result.Value.Items[1].TotalPrice);
    // }

    // [Fact]
    // public async Task ShouldHandleALargeNumberOfUsersAndCartsWithoutPerformanceDegradation()
    // {
    //     // Arrange
    //     var mockCartRepository = new Mock<ICartRepository>();
    //     var userId = UserId.Create("test-user-id");
    //     var expectedCart = new Cart(userId);

    //     mockCartRepository
    //         .Setup(x => x.GetCartByUserIdAsync(It.IsAny<UserId>(), It.IsAny<CancellationToken>()))
    //         .ReturnsAsync(expectedCart);

    //     var sut = new GetCartQueryHandler(mockCartRepository.Object);
    //     var query = new GetCartQuery { UserId = userId.Value };

    //     // Act
    //     var result = await sut.Handle(query, CancellationToken.None);

    //     // Assert
    //     Assert.True(result.IsSuccess);
    //     Assert.Equal(expectedCart, result.Value);
    // }

    // [Fact]
    // public async Task ShouldReturnCartWithCorrectItems_WhenItemsAddedAndRemovedFromCart()
    // {
    //     // Arrange
    //     var userId = UserId.Create("test-user-id");
    //     var cart = new Cart(userId);
    //     cart.AddItem(new CartItem(1, "Product 1", 10, 2));
    //     cart.AddItem(new CartItem(2, "Product 2", 15, 1));
    //     cart.RemoveItem(1);

    //     _cartRepositoryMock
    //         .Setup(x => x.GetCartByUserIdAsync(userId, It.IsAny<CancellationToken>()))
    //         .ReturnsAsync(cart);

    //     var handler = new GetCartQueryHandler(_cartRepositoryMock.Object);
    //     var query = new GetCartQuery { UserId = userId.Value };

    //     // Act
    //     var result = await handler.Handle(query, CancellationToken.None);

    //     // Assert
    //     Assert.True(result.IsSuccess);
    //     Assert.NotNull(result.Value);
    //     Assert.Equal(1, result.Value.CartItems.Count);
    //     Assert.DoesNotContain(result.Value.CartItems, item => item.ProductId == 1);
    // }
}
