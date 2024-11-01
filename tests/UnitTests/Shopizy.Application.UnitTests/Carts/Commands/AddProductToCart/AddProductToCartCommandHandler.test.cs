using FluentAssertions;
using Moq;
using Shopizy.Application.Carts.Commands.AddProductToCart;
using Shopizy.Application.Common.Interfaces.Persistence;
using Shopizy.Application.Common.Wrappers;
using Shopizy.Application.UnitTests.Carts.TestUtils;
using Shopizy.Domain.Carts;
using Shopizy.Domain.Carts.Entities;
using Shopizy.Domain.Carts.ValueObjects;
using Shopizy.Domain.Common.CustomErrors;
using Shopizy.Domain.Products.ValueObjects;

namespace Shopizy.Application.UnitTests.Carts.Commands.AddProductToCart;

public class AddProductToCartCommandHandlerTests
{
    private readonly Mock<ICartRepository> _mockCartRepository;
    private readonly Mock<IProductRepository> _mockProductRepository;
    private readonly AddProductToCartCommandHandler _sut;

    public AddProductToCartCommandHandlerTests()
    {
        _mockCartRepository = new Mock<ICartRepository>();
        _mockProductRepository = new Mock<IProductRepository>();
        _sut = new AddProductToCartCommandHandler(
            _mockCartRepository.Object,
            _mockProductRepository.Object
        );
    }

    // Should return error when cart is not found
    [Fact]
    public async Task ShouldReturnErrorWhenCartIdIsInvalidAsync()
    {
        // Arrange
        var command = AddProductToCartCommandUtils.CreateCommand();

        _mockCartRepository
            .Setup(x => x.GetCartByIdAsync(It.IsAny<CartId>(), CancellationToken.None))
            .ReturnsAsync(() => null);

        // Act
        var result = (await _sut.Handle(command, CancellationToken.None)).Match(
            x => null,
            x => Result.Failure([CustomErrors.Cart.CartNotFound])
        );

        // Assert
        result.Succeeded.Should().BeFalse();
        result.Errors.Single().Should().BeEquivalentTo(CustomErrors.Cart.CartNotFound);
    }

    // Should add new line item to cart when product is not already present
    [Fact]
    public async Task ShouldAddNewLineItemToCartWhenProductIsNotAlreadyPresentAsync()
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
        _mockCartRepository.Setup(cr => cr.Commit(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        _mockCartRepository
            .Setup(cr => cr.GetCartByUserIdAsync(existingCart.UserId))
            .ReturnsAsync(existingCart);

        // Act

        var cart = (await _sut.Handle(command, CancellationToken.None)).Match(
            x => x,
            x => existingCart
        );

        // Assert
        cart.Should().NotBeNull();
        cart.Should().BeOfType<Cart>();
        cart.Should().Be(existingCart);
        cart.LineItems.Should().HaveCount(1);
        cart.LineItems[0].ProductId.Should().BeOfType(typeof(ProductId));
        cart.LineItems.Should().Contain(li => li.ProductId == ProductId.Create(command.ProductId));
        cart.LineItems[0].Quantity.Should().Be(1);
        _mockCartRepository.Verify(
            x => x.Update(It.Is<Cart>(c => c.LineItems.Count == 1)),
            Times.Once
        );
        _mockCartRepository.Verify(x => x.Commit(CancellationToken.None), Times.Once);
    }

    // Should add new line item for another product
    [Fact]
    public async Task ShouldAddNewLineItemForAnotherProductAsync()
    {
        // Arrange
        var existingCart = CartFactory.Create();
        existingCart.AddLineItem(LineItem.Create(ProductId.CreateUnique()));

        var updatedCart = CartFactory.Create();
        updatedCart.AddLineItem(LineItem.Create(ProductId.CreateUnique()));
        updatedCart.AddLineItem(CartFactory.CreateLineItem());

        var command = AddProductToCartCommandUtils.CreateCommand();

        _mockCartRepository
            .Setup(x => x.GetCartByIdAsync(It.IsAny<CartId>(), CancellationToken.None))
            .ReturnsAsync(existingCart);

        _mockProductRepository
            .Setup(x => x.IsProductExistAsync(It.IsAny<ProductId>()))
            .ReturnsAsync(true);

        _mockCartRepository.Setup(cr => cr.Update(updatedCart));
        _mockCartRepository.Setup(cr => cr.Commit(It.IsAny<CancellationToken>())).ReturnsAsync(1);
        _mockCartRepository
            .Setup(cr => cr.GetCartByUserIdAsync(updatedCart.UserId))
            .ReturnsAsync(updatedCart);

        // Act
        var cart = (await _sut.Handle(command, CancellationToken.None)).Match(
            x => x,
            x => updatedCart
        );

        // Assert
        cart.Should().NotBeNull();
        cart.Should().BeOfType<Cart>();
        cart.Should().Be(updatedCart);
        cart.LineItems.Should().HaveCount(2);
        cart.LineItems.Should().Contain(li => li.ProductId == ProductId.Create(command.ProductId));
        cart.LineItems[0].ProductId.Should().BeOfType(typeof(ProductId));
        cart.LineItems[0].Quantity.Should().Be(1);
        _mockCartRepository.Verify(
            x => x.Update(It.Is<Cart>(c => c.LineItems.Count == 2)),
            Times.Once
        );
        _mockCartRepository.Verify(x => x.Commit(CancellationToken.None), Times.Once);
        cart.LineItems.Should()
            .Contain(li => li.ProductId == ProductId.Create(command.ProductId) && li.Quantity == 1);
    }

    // [Fact]
    // public async Task ShouldReturnErrorResponseWhenProductIsOutOfStock()
    // {
    //     // Arrange
    //     var command = AddProductToCartCommandUtils.CreateCommand();

    //     _mockProductRepository
    //         .Setup(x => x.IsProductExistAsync(It.IsAny<ProductId>(), CancellationToken.None))
    //         .ReturnsAsync(true);
    //     _mockProductRepository
    //         .Setup(x => x.IsProductInStockAsync(It.IsAny<ProductId>(), CancellationToken.None))
    //         .ReturnsAsync(false);

    //     // Act
    //     var result = await _sut.Handle(cmd, CancellationToken.None);

    //     // Assert
    //     result.ShouldNotBeNull();
    //     result.IsSuccess.ShouldBeFalse();
    //     result.Errors.ShouldContain(CustomErrors.Product.ProductOutOfStock);
    // }

    // [Fact]
    // public async Task ShouldHandleConcurrentRequestsByEnsuringDataIntegrity()
    // {
    //     // Arrange
    //     var command = new AddProductToCartCommand
    //     {
    //         CartId = "cartId",
    //         ProductId = "productId",
    //         UserId = "userId",
    //     };

    //     var cart = new Cart(CartId.Create(command.CartId), UserId.Create(command.UserId));
    //     var product = true;

    //     _mockCartRepository
    //         .Setup(x => x.GetCartByIdAsync(It.IsAny<CartId>(), CancellationToken.None))
    //         .ReturnsAsync(cart);

    //     _mockProductRepository
    //         .Setup(x => x.IsProductExistAsync(It.IsAny<ProductId>(), CancellationToken.None))
    //         .ReturnsAsync(product);

    //     _mockCartRepository.Setup(x => x.Commit(It.IsAny<CancellationToken>())).ReturnsAsync(1);

    //     var tasks = new List<Task>();

    //     for (int i = 0; i < 10; i++)
    //     {
    //         tasks.Add(_sut.Handle(command, CancellationToken.None));
    //     }

    //     // Act
    //     await Task.WhenAll(tasks);

    //     // Assert
    //     _mockCartRepository.Verify(
    //         x => x.GetCartByIdAsync(It.IsAny<CartId>(), CancellationToken.None),
    //         Times.Exactly(10)
    //     );
    //     _mockProductRepository.Verify(
    //         x => x.IsProductExistAsync(It.IsAny<ProductId>(), CancellationToken.None),
    //         Times.Exactly(10)
    //     );
    //     _mockCartRepository.Verify(x => x.Update(It.IsAny<Cart>()), Times.Exactly(10));
    //     _mockCartRepository.Verify(x => x.Commit(It.IsAny<CancellationToken>()), Times.Exactly(10));
    // }

    // [Fact]
    // public async Task ShouldReturnErrorResponseWhenCartIsLockedForEditing()
    // {
    //     // Arrange
    //     var cmd = new AddProductToCartCommand
    //     {
    //         CartId = "cartId",
    //         ProductId = "productId",
    //         UserId = "userId",
    //     };
    //     _mockCartRepository
    //         .Setup(x => x.GetCartByIdAsync(It.IsAny<CartId>(), CancellationToken.None))
    //         .ReturnsAsync(new Cart(CartId.Create("cartId"), UserId.Create("userId"), true));

    //     // Act
    //     var result = await _sut.Handle(cmd, CancellationToken.None);

    //     // Assert
    //     result.ShouldNotBeNull();
    //     result.IsSuccess.ShouldBeFalse();
    //     result.Errors.ShouldContain(CustomErrors.Cart.CartLockedForEditing);
    // }

    // [Fact]
    // public async Task ShouldHandleLargeNumberOfProductsAndCartsWithoutPerformanceDegradation()
    // {
    //     // Arrange
    //     const int numberOfCarts = 1000;
    //     const int numberOfProductsPerCart = 100;

    //     var carts = new List<Cart>();
    //     for (var i = 0; i < numberOfCarts; i++)
    //     {
    //         var cart = new Cart(CartId.Create($"cart-{i}"), UserId.Create($"user-{i}"));
    //         for (var j = 0; j < numberOfProductsPerCart; j++)
    //         {
    //             cart.AddLineItem(LineItem.Create(ProductId.Create($"product-{j}")));
    //         }
    //         carts.Add(cart);
    //     }

    //     _mockCartRepository
    //         .Setup(x => x.GetCartByIdAsync(It.IsAny<CartId>(), CancellationToken.None))
    //         .ReturnsAsync((CartId cartId) => carts.FirstOrDefault(c => c.Id == cartId));

    //     _mockProductRepository
    //         .Setup(x => x.IsProductExistAsync(It.IsAny<ProductId>(), CancellationToken.None))
    //         .ReturnsAsync(true);

    //     _mockCartRepository.Setup(x => x.Commit(It.IsAny<CancellationToken>())).ReturnsAsync(1);

    //     // Act
    //     var tasks = new List<Task<IResult<Cart>>>();
    //     for (var i = 0; i < numberOfCarts; i++)
    //     {
    //         var cmd = new AddProductToCartCommand
    //         {
    //             CartId = $"cart-{i}",
    //             ProductId = $"product-{numberOfProductsPerCart}",
    //             UserId = $"user-{i}",
    //         };
    //         tasks.Add(_sut.Handle(cmd, CancellationToken.None));
    //     }

    //     await Task.WhenAll(tasks);

    //     // Assert
    //     foreach (var task in tasks)
    //     {
    //         var result = await task;
    //         result.IsSuccess.ShouldBeTrue();
    //         result.Value.LineItems.Count.ShouldBe(numberOfProductsPerCart + 1);
    //     }
    // }

    // [Fact]
    // public async Task ShouldSupportPaginationWhenRetrievingCarts()
    // {
    //     // Arrange
    //     var cmd = new AddProductToCartCommand
    //     {
    //         CartId = "cartId",
    //         ProductId = "productId",
    //         UserId = "userId",
    //     };
    //     var pageNumber = 1;
    //     var pageSize = 10;

    //     _mockCartRepository
    //         .Setup(x =>
    //             x.GetCartsByUserIdAsync(
    //                 It.IsAny<UserId>(),
    //                 pageNumber,
    //                 pageSize,
    //                 CancellationToken.None
    //             )
    //         )
    //         .ReturnsAsync(
    //             new List<Cart> { new Cart(CartId.Create("cartId"), UserId.Create("userId")) }
    //         );

    //     // Act
    //     var result = await _sut.Handle(cmd, CancellationToken.None);

    //     // Assert
    //     _mockCartRepository.Verify(
    //         x =>
    //             x.GetCartsByUserIdAsync(
    //                 It.IsAny<UserId>(),
    //                 pageNumber,
    //                 pageSize,
    //                 CancellationToken.None
    //             ),
    //         Times.Once
    //     );
    //     Assert.True(result.IsSuccess);
    //     Assert.NotEmpty(result.Value);
    // }

    // [Fact]
    // public async Task ShouldValidateUserAuthorizationBeforeAllowingCartModifications()
    // {
    //     // Arrange
    //     var command = new AddProductToCartCommand
    //     {
    //         CartId = "valid-cart-id",
    //         ProductId = "valid-product-id",
    //         UserId = "valid-user-id",
    //     };

    //     _authorizationService
    //         .Setup(x => x.AuthorizeAsync(It.IsAny<ClaimsPrincipal>(), It.IsAny<string>()))
    //         .ReturnsAsync(AuthorizationResult.Success());

    //     // Act
    //     var result = await _sut.Handle(command, CancellationToken.None);

    //     // Assert
    //     _authorizationService.Verify(
    //         x => x.AuthorizeAsync(It.IsAny<ClaimsPrincipal>(), It.IsAny<string>()),
    //         Times.Once
    //     );

    //     // Additional assertions for successful authorization
    //     Assert.True(result.IsSuccess);
    // }

    // [Fact]
    // public async Task ShouldReturnErrorResponseWhenProductIdIsNotFound()
    // {
    //     // Arrange
    //     var cmd = new AddProductToCartCommand
    //     {
    //         CartId = "cartId",
    //         ProductId = "productId",
    //         UserId = "userId",
    //     };
    //     _mockProductRepository
    //         .Setup(x => x.IsProductExistAsync(It.IsAny<ProductId>(), CancellationToken.None))
    //         .ReturnsAsync(false);

    //     // Act
    //     var result = await _sut.Handle(cmd, CancellationToken.None);

    //     // Assert
    //     result.ShouldNotBeNull();
    //     result.IsSuccess.ShouldBeFalse();
    //     result.Errors.ShouldContain(CustomErrors.Product.ProductNotFound);
    // }
}