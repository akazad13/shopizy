using Moq;
using Shopizy.Application.Carts.Commands.AddProductToCart;
using Shopizy.Application.Common.Interfaces.Persistence;
using Shopizy.Application.Common.Security.CurrentUser;
using Shopizy.Application.UnitTests.Carts.TestUtils;
using Shopizy.Application.UnitTests.TestUtils.Constants;
using Shopizy.Domain.Carts;
using Shopizy.Domain.Carts.Entities;
using Shopizy.Domain.Carts.ValueObjects;
using Shopizy.Domain.Common.CustomErrors;
using Shopizy.Domain.Products.ValueObjects;
using Shopizy.Domain.Users.ValueObjects;

namespace Shopizy.Application.UnitTests.Carts.Commands.AddProductToCart;

public class AddProductToCartCommandHandlerTests
{
    private readonly Mock<ICartRepository> _mockCartRepository;
    private readonly Mock<IProductRepository> _mockProductRepository;
    private readonly Mock<ICurrentUser> _mockCurrentUser;
    private readonly AddProductToCartCommandHandler _sut;

    public AddProductToCartCommandHandlerTests()
    {
        _mockCartRepository = new Mock<ICartRepository>();
        _mockProductRepository = new Mock<IProductRepository>();
        _mockCurrentUser = new Mock<ICurrentUser>();

        _sut = new AddProductToCartCommandHandler(
            _mockCartRepository.Object,
            _mockProductRepository.Object,
            _mockCurrentUser.Object
        );
    }

    // Should return error when cart is not found
    [Fact]
    public async Task Should_ReturnError_WhenCartIdIsInvalid()
    {
        // Arrange
        var command = AddProductToCartCommandUtils.CreateCommand();

        _mockCartRepository
            .Setup(x => x.GetCartByIdAsync(It.IsAny<CartId>(), CancellationToken.None))
            .ReturnsAsync(() => null);

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsError);
        Assert.NotNull(result.Errors);
        Assert.Single(result.Errors);
        Assert.Equal(CustomErrors.Cart.CartNotFound, result.Errors[0]);
    }

    // Should add new line item to cart when product is not already present
    [Fact]
    public async Task Should_AddNewLineItemToCart_WhenProductIsNotAlreadyPresent()
    {
        // Arrange
        var existingCart = CartFactory.Create();
        var command = AddProductToCartCommandUtils.CreateCommand();

        _mockCartRepository
            .Setup(x => x.GetCartByIdAsync(It.IsAny<CartId>(), CancellationToken.None))
            .ReturnsAsync(existingCart);
        _mockProductRepository
            .Setup(x => x.IsProductExistAsync(It.IsAny<ProductId>()))
            .ReturnsAsync(true);

        _mockCartRepository.Setup(cr => cr.Update(existingCart));

        _mockCartRepository
            .Setup(cr => cr.GetCartByUserIdAsync(existingCart.UserId))
            .ReturnsAsync(existingCart);

        _mockCurrentUser.Setup(cu => cu.GetCurrentUserId()).Returns(Constants.User.Id.Value);

        // Act
        var cart = await _sut.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(cart.IsError);
        Assert.NotNull(cart.Value);
        Assert.IsType<Cart>(cart.Value);
        Assert.Equal(existingCart, cart.Value);
        Assert.Single(cart.Value.CartItems);
        Assert.IsType<ProductId>(cart.Value.CartItems[0].ProductId);
        Assert.Contains(cart.Value.CartItems, li => li.ProductId == ProductId.Create(command.ProductId));
        Assert.Equal(command.Quantity, cart.Value.CartItems[0].Quantity);
        _mockCartRepository.Verify(
            x => x.Update(It.Is<Cart>(c => c.CartItems.Count == 1)),
            Times.Once
        );
    }

    // Should add new line item for another product
    [Fact]
    public async Task Should_AddNewLineItem_ForAnotherProduct()
    {
        // Arrange
        var existingCart = CartFactory.Create();
        existingCart.AddLineItem(
            CartItem.Create(
                ProductId.CreateUnique(),
                Constants.CartItem.Color,
                Constants.CartItem.Size,
                1
            )
        );

        var updatedCart = CartFactory.Create();
        updatedCart.AddLineItem(
            CartItem.Create(
                ProductId.CreateUnique(),
                Constants.CartItem.Color,
                Constants.CartItem.Size,
                1
            )
        );
        updatedCart.AddLineItem(CartFactory.CreateCartItem());

        var command = AddProductToCartCommandUtils.CreateCommand();

        _mockCartRepository
            .Setup(x => x.GetCartByIdAsync(It.IsAny<CartId>(), CancellationToken.None))
            .ReturnsAsync(existingCart);

        _mockProductRepository
            .Setup(x => x.IsProductExistAsync(It.IsAny<ProductId>()))
            .ReturnsAsync(true);

        _mockCartRepository.Setup(cr => cr.Update(updatedCart));
        _mockCartRepository
            .Setup(cr => cr.GetCartByUserIdAsync(updatedCart.UserId))
            .ReturnsAsync(updatedCart);

        _mockCurrentUser.Setup(cu => cu.GetCurrentUserId()).Returns(Constants.User.Id.Value);

        // Act
        var cart = await _sut.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(cart.IsError);
        Assert.NotNull(cart.Value);
        Assert.IsType<Cart>(cart.Value);
        Assert.Equal(updatedCart, cart.Value);
        Assert.Equal(2, cart.Value.CartItems.Count);
        Assert.Contains(cart.Value.CartItems, li => li.ProductId == ProductId.Create(command.ProductId));
        Assert.IsType<ProductId>(cart.Value.CartItems[0].ProductId);
        Assert.Equal(1, cart.Value.CartItems[0].Quantity);
        _mockCartRepository.Verify(
            x => x.Update(It.Is<Cart>(c => c.CartItems.Count == 2)),
            Times.Once
        );
        Assert.Contains(cart.Value.CartItems, li => li.ProductId == ProductId.Create(command.ProductId) && li.Quantity == 1);
    }
}
