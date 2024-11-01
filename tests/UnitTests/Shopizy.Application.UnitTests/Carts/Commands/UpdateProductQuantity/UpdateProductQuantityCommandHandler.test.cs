using FluentAssertions;
using Moq;
using Shopizy.Application.Carts.Commands.UpdateProductQuantity;
using Shopizy.Application.Common.Interfaces.Persistence;
using Shopizy.Application.Common.Wrappers;
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
    public async Task ShouldReturnsCartNotFoundWhenCartIsNoFoundAsync()
    {
        // Arrange
        var command = UpdateProductQuantityCommandUtils.CreateCommand(37);

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

    // Should update the quantity correctly when the product exists in the cart
    // [Fact]
    // public async Task ShouldUpdateQuantityCorrectlyWhenProductExistsInCartAsync()
    // {
    //     // Arrange
    //     var command = UpdateProductQuantityCommandUtils.CreateCommand(37);
    //     var cart = CartFactory.Create();
    //     cart.UpdateLineItem(ProductId.Create(command.ProductId), 2);
    //     _mockCartRepository
    //         .Setup(x => x.GetCartByIdAsync(It.IsAny<CartId>(), It.IsAny<CancellationToken>()))
    //         .ReturnsAsync(cart);

    //     // Act
    //     var result = await _sut.Handle(command, CancellationToken.None);

    //     // Assert
    //     Assert.IsType<Response<GenericResponse>>(result);
    //     Assert.True(result.IsSuccess);
    //     Assert.Equal("successfully updated cart.", result.Data.Message);
    //     cartRepositoryMock.Verify(
    //         x =>
    //             x.Update(
    //                 It.Is<Cart>(c => c.GetLineItem(ProductId.Create("product-id")).Quantity == 3)
    //             ),
    //         Times.Once
    //     );
    //     cartRepositoryMock.Verify(x => x.Commit(It.IsAny<CancellationToken>()), Times.Once);
    // }

    // Should not update the quantity when the product does not exist in the cart
    // [Fact]
    // public async Task ShouldNotUpdateQuantityWhenProductDoesNotExistInCart()
    // {
    //     // Arrange
    //     var cartRepositoryMock = new Mock<ICartRepository>();
    //     var cart = new Cart(CartId.Create("cart-id"));
    //     cartRepositoryMock
    //         .Setup(x => x.GetCartByIdAsync(It.IsAny<CartId>(), It.IsAny<CancellationToken>()))
    //         .ReturnsAsync(cart);

    //     var handler = new UpdateProductQuantityCommandHandler(cartRepositoryMock.Object);
    //     var command = new UpdateProductQuantityCommand
    //     {
    //         CartId = "cart-id",
    //         ProductId = "non-existent-product-id",
    //         Quantity = 1,
    //     };

    //     // Act
    //     var result = await handler.Handle(command, CancellationToken.None);

    //     // Assert
    //     Assert.IsType<Response<GenericResponse>>(result);
    //     Assert.False(result.IsSuccess);
    //     Assert.Equal(CustomErrors.Cart.CartPrductNotAdded, result.Errors.First());
    // }

    // // Should handle concurrent updates to the same cart without data loss
    // [Fact]
    // public async Task ShouldHandleConcurrentUpdatesToTheSameCartWithoutDataLoss()
    // {
    //     // Arrange
    //     var cartRepositoryMock = new Mock<ICartRepository>();
    //     var cartId = "cart-id";
    //     var productId = "product-id";
    //     var quantity = 1;

    //     var existingCart = new Cart(CartId.Create(cartId));
    //     existingCart.AddLineItem(ProductId.Create(productId), quantity);

    //     cartRepositoryMock
    //         .Setup(repo => repo.GetCartByIdAsync(It.IsAny<CartId>(), It.IsAny<CancellationToken>()))
    //         .ReturnsAsync(existingCart);

    //     cartRepositoryMock
    //         .Setup(repo => repo.Update(It.IsAny<Cart>()))
    //         .Callback<Cart>(cart =>
    //         {
    //             cart.UpdateLineItem(ProductId.Create(productId), quantity + 1);
    //         });

    //     cartRepositoryMock
    //         .Setup(repo => repo.Commit(It.IsAny<CancellationToken>()))
    //         .ReturnsAsync(1);

    //     var handler = new UpdateProductQuantityCommandHandler(cartRepositoryMock.Object);
    //     var command = new UpdateProductQuantityCommand
    //     {
    //         CartId = cartId,
    //         ProductId = productId,
    //         Quantity = quantity + 1,
    //     };

    //     // Act
    //     var result1 = await handler.Handle(command, CancellationToken.None);
    //     var result2 = await handler.Handle(command, CancellationToken.None);

    //     // Assert
    //     Assert.True(result1.IsSuccess);
    //     Assert.True(result2.IsSuccess);
    //     cartRepositoryMock.Verify(repo => repo.Update(It.IsAny<Cart>()), Times.Exactly(2));
    //     cartRepositoryMock.Verify(
    //         repo => repo.Commit(It.IsAny<CancellationToken>()),
    //         Times.Exactly(2)
    //     );
    // }

    // // Should return an error response when the cart repository fails to update the cart
    // [Fact]
    // public async Task ShouldReturnErrorResponseWhenCartRepositoryFailsToUpdateCart()
    // {
    //     // Arrange
    //     var cartRepositoryMock = new Mock<ICartRepository>();
    //     cartRepositoryMock
    //         .Setup(x => x.GetCartByIdAsync(It.IsAny<CartId>(), It.IsAny<CancellationToken>()))
    //         .ReturnsAsync(new Cart(CartId.Create("cart-id")));
    //     cartRepositoryMock.Setup(x => x.Commit(It.IsAny<CancellationToken>())).ReturnsAsync(0);

    //     var handler = new UpdateProductQuantityCommandHandler(cartRepositoryMock.Object);
    //     var command = new UpdateProductQuantityCommand
    //     {
    //         CartId = "cart-id",
    //         ProductId = "product-id",
    //         Quantity = 1,
    //     };

    //     // Act
    //     var result = await handler.Handle(command, CancellationToken.None);

    //     // Assert
    //     Assert.IsType<Response<GenericResponse>>(result);
    //     Assert.False(result.IsSuccess);
    //     Assert.Equal(CustomErrors.Cart.CartPrductNotAdded, result.Errors.First());
    // }

    // // Should return an error response when the cart repository fails to commit changes
    // [Fact]
    // public async Task ShouldReturnErrorResponseWhenCartRepositoryFailsToCommitChanges()
    // {
    //     // Arrange
    //     var cartRepositoryMock = new Mock<ICartRepository>();
    //     cartRepositoryMock
    //         .Setup(x => x.GetCartByIdAsync(It.IsAny<CartId>(), It.IsAny<CancellationToken>()))
    //         .ReturnsAsync(new Domain.Carts.Cart(CartId.Create("cart-id"), new List<LineItem>()));
    //     cartRepositoryMock.Setup(x => x.Commit(It.IsAny<CancellationToken>())).ReturnsAsync(0);

    //     var handler = new UpdateProductQuantityCommandHandler(cartRepositoryMock.Object);
    //     var command = new UpdateProductQuantityCommand
    //     {
    //         CartId = "cart-id",
    //         ProductId = "product-id",
    //         Quantity = 1,
    //     };

    //     // Act
    //     var result = await handler.Handle(command, CancellationToken.None);

    //     // Assert
    //     Assert.IsType<Response<GenericResponse>>(result);
    //     Assert.False(result.IsSuccess);
    //     Assert.Equal(CustomErrors.Cart.CartPrductNotAdded, result.Errors.First());
    // }

    // // Should handle negative quantities by removing the line item
    // [Fact]
    // public async Task ShouldHandleNegativeQuantitiesByRemovingTheLineItem()
    // {
    //     // Arrange
    //     var cartRepositoryMock = new Mock<ICartRepository>();
    //     var cart = new Cart(CartId.Create("cart-id"));
    //     cart.AddLineItem(ProductId.Create("product-id"), 5);
    //     cartRepositoryMock
    //         .Setup(x => x.GetCartByIdAsync(It.IsAny<CartId>(), It.IsAny<CancellationToken>()))
    //         .ReturnsAsync(cart);

    //     var handler = new UpdateProductQuantityCommandHandler(cartRepositoryMock.Object);
    //     var command = new UpdateProductQuantityCommand
    //     {
    //         CartId = "cart-id",
    //         ProductId = "product-id",
    //         Quantity = -3,
    //     };

    //     // Act
    //     var result = await handler.Handle(command, CancellationToken.None);

    //     // Assert
    //     Assert.IsType<Response<GenericResponse>>(result);
    //     Assert.True(result.IsSuccess);
    //     Assert.Empty(result.Errors);
    //     cartRepositoryMock.Verify(
    //         x => x.Update(It.Is<Cart>(c => c.GetLineItem(ProductId.Create("product-id")) == null)),
    //         Times.Once
    //     );
    //     cartRepositoryMock.Verify(x => x.Commit(It.IsAny<CancellationToken>()), Times.Once);
    // }

    // // Should handle zero quantities by removing the line item
    // [Fact]
    // public async Task ShouldHandleZeroQuantitiesByRemovingTheLineItem()
    // {
    //     // Arrange
    //     var cartRepositoryMock = new Mock<ICartRepository>();
    //     var cartId = "cart-id";
    //     var productId = "product-id";
    //     var cart = new Cart(CartId.Create(cartId));
    //     cart.AddLineItem(ProductId.Create(productId), 5);
    //     cartRepositoryMock
    //         .Setup(x => x.GetCartByIdAsync(It.IsAny<CartId>(), It.IsAny<CancellationToken>()))
    //         .ReturnsAsync(cart);
    //     var handler = new UpdateProductQuantityCommandHandler(cartRepositoryMock.Object);
    //     var command = new UpdateProductQuantityCommand
    //     {
    //         CartId = cartId,
    //         ProductId = productId,
    //         Quantity = 0,
    //     };

    //     // Act
    //     var result = await handler.Handle(command, CancellationToken.None);

    //     // Assert
    //     Assert.IsType<Response<GenericResponse>>(result);
    //     Assert.True(result.IsSuccess);
    //     Assert.DoesNotContain(cart.LineItems, li => li.ProductId == productId);
    // }

    // // Should handle decimal quantities by rounding them to the nearest whole number
    // [Fact]
    // public async Task ShouldHandleDecimalQuantitiesByRoundingToNearestWholeNumber()
    // {
    //     // Arrange
    //     var cartRepositoryMock = new Mock<ICartRepository>();
    //     var cart = new Cart(CartId.Create("cart-id"));
    //     cart.AddLineItem(ProductId.Create("product-id"), 10.5m);
    //     cartRepositoryMock
    //         .Setup(x => x.GetCartByIdAsync(It.IsAny<CartId>(), It.IsAny<CancellationToken>()))
    //         .ReturnsAsync(cart);
    //     var handler = new UpdateProductQuantityCommandHandler(cartRepositoryMock.Object);
    //     var command = new UpdateProductQuantityCommand
    //     {
    //         CartId = "cart-id",
    //         ProductId = "product-id",
    //         Quantity = 15.7m,
    //     };

    //     // Act
    //     var result = await handler.Handle(command, CancellationToken.None);

    //     // Assert
    //     Assert.IsType<Response<GenericResponse>>(result);
    //     Assert.True(result.IsSuccess);
    //     Assert.Equal("successfully updated cart.", result.Data.Message);
    //     Assert.Equal(16, cart.LineItems.First().Quantity);
    // }

    // // Should handle large quantities by updating the line item accordingly
    // [Fact]
    // public async Task ShouldHandleLargeQuantitiesByUpdatingTheLineItemAccordingly()
    // {
    //     // Arrange
    //     var cartRepositoryMock = new Mock<ICartRepository>();
    //     var cartId = "cart-id";
    //     var productId = "product-id";
    //     var largeQuantity = 1000;

    //     var cart = new Cart(CartId.Create(cartId));
    //     cart.AddLineItem(ProductId.Create(productId), 1);

    //     cartRepositoryMock
    //         .Setup(x => x.GetCartByIdAsync(It.IsAny<CartId>(), It.IsAny<CancellationToken>()))
    //         .ReturnsAsync(cart);

    //     var handler = new UpdateProductQuantityCommandHandler(cartRepositoryMock.Object);
    //     var command = new UpdateProductQuantityCommand
    //     {
    //         CartId = cartId,
    //         ProductId = productId,
    //         Quantity = largeQuantity,
    //     };

    //     // Act
    //     var result = await handler.Handle(command, CancellationToken.None);

    //     // Assert
    //     Assert.IsType<Response<GenericResponse>>(result);
    //     Assert.True(result.IsSuccess);
    //     Assert.Equal(largeQuantity, cart.GetLineItem(ProductId.Create(productId)).Quantity);
    //     cartRepositoryMock.Verify(x => x.Update(cart), Times.Once);
    //     cartRepositoryMock.Verify(x => x.Commit(It.IsAny<CancellationToken>()), Times.Once);
    // }
}
