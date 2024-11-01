using FluentAssertions;
using Moq;
using Shopizy.Application.Carts.Commands.RemoveProductFromCart;
using Shopizy.Application.Common.Interfaces.Persistence;
using Shopizy.Application.Common.Wrappers;
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

    // Should return cart not found when cart  is not found
    [Fact]
    public async Task ShouldReturnsCartNotFoundWhenCartIsNotFoundAsync()
    {
        // Arrange
        var command = RemoveProductFromCartCommandUtils.CreateCommand();

        _mockCartRepository
            .Setup(cr => cr.GetCartByIdAsync(CartId.Create(command.CartId), CancellationToken.None))
            .ReturnsAsync(() => null);

        // Act
        var result = (await _sut.Handle(command, default)).Match(
            x => null,
            x => Result.Failure([CustomErrors.Cart.CartNotFound])
        );

        // Assert
        result.Succeeded.Should().BeFalse();
        result.Errors.Single().Should().BeEquivalentTo(CustomErrors.Cart.CartNotFound);

        _mockCartRepository.Verify(
            cr => cr.GetCartByIdAsync(CartId.Create(command.CartId), CancellationToken.None),
            Times.Once
        );
        _mockCartRepository.Verify(x => x.Update(It.IsAny<Cart>()), Times.Never);
        _mockCartRepository.Verify(x => x.Commit(It.IsAny<CancellationToken>()), Times.Never);
    }

    // Should remove last line item from cart and commit changes when cart is found
    [Fact]
    public async Task ShouldRemoveLastLineItemFromCartWhenCartIsFoundAsync()
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
        var result = (await _sut.Handle(command, CancellationToken.None)).Match(
            x => x.Message,
            x => null
        );

        // Assert
        result.Should().BeEquivalentTo("successfully removed product from cart.");
        _mockCartRepository.Verify(
            cr => cr.GetCartByIdAsync(CartId.Create(command.CartId), CancellationToken.None),
            Times.Once
        );
        _mockCartRepository.Verify(
            x => x.Update(It.Is<Cart>(c => c.LineItems.Count == 0)),
            Times.Once
        );
        _mockCartRepository.Verify(x => x.Commit(It.IsAny<CancellationToken>()), Times.Once);
    }

    // [Fact]
    // public async Task ShouldReturnErrorResponseWhenProductNotFoundInCart()
    // {
    //     // Arrange
    //     var cartId = Guid.NewGuid();
    //     var productId = 1;
    //     var cart = new Cart(
    //         CartId.Create(cartId),
    //         new List<LineItem> { new LineItem(2, 1, 100) } // Different product id
    //     );
    //     _mockCartRepository
    //         .Setup(x => x.GetCartByIdAsync(It.IsAny<CartId>(), It.IsAny<CancellationToken>()))
    //         .ReturnsAsync(cart);
    //     var command = new RemoveProductFromCartCommand { CartId = cartId, ProductId = productId };
    //     var handler = new RemoveProductFromCartCommandHandler(_mockCartRepository.Object);

    //     // Act
    //     var result = await handler.Handle(command, CancellationToken.None);

    //     // Assert
    //     Assert.False(result.IsSuccess);
    //     Assert.Equal(CustomErrors.Cart.CartNotFound.Code, result.Error.Code);
    //     Assert.Equal(CustomErrors.Cart.CartNotFound.Message, result.Error.Message);
    //     _mockCartRepository.Verify(x => x.Update(It.IsAny<Cart>()), Times.Never);
    //     _mockCartRepository.Verify(x => x.Commit(It.IsAny<CancellationToken>()), Times.Never);
    // }

    // [Fact]
    // public async Task ShouldHandleConcurrentUpdatesToCartByEnsuringDataConsistency()
    // {
    //     // Arrange
    //     var cartId = Guid.NewGuid();
    //     var productId = 1;
    //     var cart = new Cart(
    //         CartId.Create(cartId),
    //         new List<LineItem> { new LineItem(productId, 1, 100) }
    //     );
    //     var cartRepositoryMock = new Mock<ICartRepository>();
    //     cartRepositoryMock
    //         .Setup(x => x.GetCartByIdAsync(It.IsAny<CartId>(), It.IsAny<CancellationToken>()))
    //         .ReturnsAsync(cart);
    //     cartRepositoryMock
    //         .Setup(x => x.Commit(It.IsAny<CancellationToken>()))
    //         .ReturnsAsync(1)
    //         .Callback(() =>
    //         {
    //             // Simulate concurrent update
    //             cart.AddLineItem(new LineItem(productId + 1, 1, 150));
    //         });
    //     var command = new RemoveProductFromCartCommand { CartId = cartId, ProductId = productId };
    //     var handler = new RemoveProductFromCartCommandHandler(cartRepositoryMock.Object);

    //     // Act
    //     var result = await handler.Handle(command, CancellationToken.None);

    //     // Assert
    //     Assert.True(result.IsSuccess);
    //     Assert.Equal("successfully removed product from cart.", result.Value.Message);
    //     cartRepositoryMock.Verify(
    //         x =>
    //             x.Update(
    //                 It.Is<Cart>(c =>
    //                     c.LineItems.Count == 1
    //                     && c.LineItems.First().ProductId.Value == productId + 1
    //                 )
    //             ),
    //         Times.Once
    //     );
    //     cartRepositoryMock.Verify(x => x.Commit(It.IsAny<CancellationToken>()), Times.Once);
    // }

    // [Fact]
    // public async Task ShouldValidateProductQuantityBeforeRemovingFromCart()
    // {
    //     // Arrange
    //     var cartId = Guid.NewGuid();
    //     var productId = 1;
    //     var cart = new Cart(
    //         CartId.Create(cartId),
    //         new List<LineItem> { new LineItem(productId, 2, 100) }
    //     );
    //     _mockCartRepository
    //         .Setup(x => x.GetCartByIdAsync(It.IsAny<CartId>(), It.IsAny<CancellationToken>()))
    //         .ReturnsAsync(cart);
    //     _mockCartRepository.Setup(x => x.Commit(It.IsAny<CancellationToken>())).ReturnsAsync(1);
    //     var command = new RemoveProductFromCartCommand
    //     {
    //         CartId = cartId,
    //         ProductId = productId,
    //         Quantity = 3, // Trying to remove more quantity than available
    //     };
    //     var handler = new RemoveProductFromCartCommandHandler(_mockCartRepository.Object);

    //     // Act
    //     var result = await handler.Handle(command, CancellationToken.None);

    //     // Assert
    //     Assert.False(result.IsSuccess);
    //     Assert.Equal(CustomErrors.Cart.InsufficientProductQuantity, result.Errors.First());
    //     _mockCartRepository.Verify(x => x.Update(It.IsAny<Cart>()), Times.Never);
    //     _mockCartRepository.Verify(x => x.Commit(It.IsAny<CancellationToken>()), Times.Never);
    // }

    // [Fact]
    // public async Task ShouldHandleScenariosWhereCartIsEmpty()
    // {
    //     // Arrange
    //     var cartId = Guid.NewGuid();
    //     var productId = 1;
    //     var cart = new Cart(CartId.Create(cartId), new List<LineItem>());
    //     _mockCartRepository
    //         .Setup(x => x.GetCartByIdAsync(It.IsAny<CartId>(), It.IsAny<CancellationToken>()))
    //         .ReturnsAsync(cart);
    //     _mockCartRepository.Setup(x => x.Commit(It.IsAny<CancellationToken>())).ReturnsAsync(1);
    //     var command = new RemoveProductFromCartCommand { CartId = cartId, ProductId = productId };
    //     var handler = new RemoveProductFromCartCommandHandler(_mockCartRepository.Object);

    //     // Act
    //     var result = await handler.Handle(command, CancellationToken.None);

    //     // Assert
    //     Assert.True(result.IsSuccess);
    //     Assert.Equal("successfully removed product from cart.", result.Value.Message);
    //     _mockCartRepository.Verify(
    //         x => x.Update(It.Is<Cart>(c => c.LineItems.Count == 0)),
    //         Times.Never
    //     );
    //     _mockCartRepository.Verify(x => x.Commit(It.IsAny<CancellationToken>()), Times.Never);
    // }

    // [Fact]
    // public async Task ShouldReturnErrorWhenProductIsOutOfStock()
    // {
    //     // Arrange
    //     var cartId = Guid.NewGuid();
    //     var productId = 1;
    //     var cart = new Cart(
    //         CartId.Create(cartId),
    //         new List<LineItem> { new LineItem(productId, 1, 0) }
    //     );
    //     _mockCartRepository
    //         .Setup(x => x.GetCartByIdAsync(It.IsAny<CartId>(), It.IsAny<CancellationToken>()))
    //         .ReturnsAsync(cart);
    //     var command = new RemoveProductFromCartCommand { CartId = cartId, ProductId = productId };
    //     var handler = new RemoveProductFromCartCommandHandler(_mockCartRepository.Object);

    //     // Act
    //     var result = await handler.Handle(command, CancellationToken.None);

    //     // Assert
    //     Assert.False(result.IsSuccess);
    //     Assert.Equal(CustomErrors.Cart.ProductOutOfStock, result.Value.Errors.First());
    //     _mockCartRepository.Verify(x => x.Update(It.IsAny<Cart>()), Times.Never);
    //     _mockCartRepository.Verify(x => x.Commit(It.IsAny<CancellationToken>()), Times.Never);
    // }

    // [Fact]
    // public async Task ShouldHandleScenariosWhereCartHasMaxNumberOfProducts()
    // {
    //     // Arrange
    //     var cartId = Guid.NewGuid();
    //     var maxNumberOfProducts = 10; // Define the maximum number of products allowed in a cart
    //     var cart = new Cart(
    //         CartId.Create(cartId),
    //         Enumerable
    //             .Range(1, maxNumberOfProducts)
    //             .Select(productId => new LineItem(productId, 1, 100))
    //             .ToList()
    //     );
    //     _mockCartRepository
    //         .Setup(x => x.GetCartByIdAsync(It.IsAny<CartId>(), It.IsAny<CancellationToken>()))
    //         .ReturnsAsync(cart);
    //     _mockCartRepository.Setup(x => x.Commit(It.IsAny<CancellationToken>())).ReturnsAsync(1);
    //     var command = new RemoveProductFromCartCommand
    //     {
    //         CartId = cartId,
    //         ProductId = 1, // Remove the first product
    //     };
    //     var handler = new RemoveProductFromCartCommandHandler(_mockCartRepository.Object);

    //     // Act
    //     var result = await handler.Handle(command, CancellationToken.None);

    //     // Assert
    //     Assert.True(result.IsSuccess);
    //     Assert.Equal("successfully removed product from cart.", result.Value.Message);
    //     _mockCartRepository.Verify(
    //         x => x.Update(It.Is<Cart>(c => c.LineItems.Count == maxNumberOfProducts - 1)),
    //         Times.Once
    //     );
    //     _mockCartRepository.Verify(x => x.Commit(It.IsAny<CancellationToken>()), Times.Once);
    // }

    // [Fact]
    // public async Task ShouldHandleCartWithMinimumQuantityRequirement()
    // {
    //     // Arrange
    //     var cartId = Guid.NewGuid();
    //     var productId = 1;
    //     var cart = new Cart(
    //         CartId.Create(cartId),
    //         new List<LineItem> { new LineItem(productId, 1, 100) }
    //     );
    //     cart.SetMinimumQuantityRequirement(productId, 2);
    //     _mockCartRepository
    //         .Setup(x => x.GetCartByIdAsync(It.IsAny<CartId>(), It.IsAny<CancellationToken>()))
    //         .ReturnsAsync(cart);
    //     _mockCartRepository.Setup(x => x.Commit(It.IsAny<CancellationToken>())).ReturnsAsync(1);
    //     var command = new RemoveProductFromCartCommand { CartId = cartId, ProductId = productId };
    //     var handler = new RemoveProductFromCartCommandHandler(_mockCartRepository.Object);

    //     // Act
    //     var result = await handler.Handle(command, CancellationToken.None);

    //     // Assert
    //     Assert.False(result.IsSuccess);
    //     Assert.Equal(CustomErrors.Cart.MinimumQuantityNotMet, result.Errors.First());
    //     _mockCartRepository.Verify(x => x.Update(It.IsAny<Cart>()), Times.Never);
    //     _mockCartRepository.Verify(x => x.Commit(It.IsAny<CancellationToken>()), Times.Never);
    // }

    // [Fact]
    // public async Task ShouldHandleCartWithMaxQuantityRequirement()
    // {
    //     // Arrange
    //     var cartId = Guid.NewGuid();
    //     var productId = 1;
    //     var maxQuantity = 2;
    //     var cart = new Cart(
    //         CartId.Create(cartId),
    //         new List<LineItem> { new LineItem(productId, maxQuantity, 100) }
    //     );
    //     _mockCartRepository
    //         .Setup(x => x.GetCartByIdAsync(It.IsAny<CartId>(), It.IsAny<CancellationToken>()))
    //         .ReturnsAsync(cart);
    //     _mockCartRepository.Setup(x => x.Commit(It.IsAny<CancellationToken>())).ReturnsAsync(1);
    //     var command = new RemoveProductFromCartCommand { CartId = cartId, ProductId = productId };
    //     var handler = new RemoveProductFromCartCommandHandler(_mockCartRepository.Object);

    //     // Act
    //     var result = await handler.Handle(command, CancellationToken.None);

    //     // Assert
    //     Assert.True(result.IsSuccess);
    //     Assert.Equal("successfully removed product from cart.", result.Value.Message);
    //     _mockCartRepository.Verify(
    //         x => x.Update(It.Is<Cart>(c => c.LineItems.Count == 0)),
    //         Times.Never
    //     );
    //     _mockCartRepository.Verify(x => x.Commit(It.IsAny<CancellationToken>()), Times.Never);
    // }
}
