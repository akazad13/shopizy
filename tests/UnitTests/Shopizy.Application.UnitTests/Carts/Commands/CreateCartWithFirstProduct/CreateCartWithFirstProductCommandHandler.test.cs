using FluentAssertions;
using Moq;
using Shopizy.Application.Carts.Commands.CreateCartWithFirstProduct;
using Shopizy.Application.Common.Interfaces.Persistence;
using Shopizy.Application.Common.Security.CurrentUser;
using Shopizy.Application.UnitTests.Carts.TestUtils;
using Shopizy.Application.UnitTests.TestUtils.Constants;
using Shopizy.Domain.Carts;
using Shopizy.Domain.Common.CustomErrors;
using Shopizy.Domain.Products.ValueObjects;

namespace Shopizy.Application.UnitTests.Carts.Commands.CreateCartWithFirstProduct;

public class CreateCartWithFirstProductCommandHandlerTests
{
    private readonly CreateCartWithFirstProductCommandHandler _sut;
    private readonly Mock<IProductRepository> _mockProductRepository;
    private readonly Mock<ICartRepository> _mockCartRepository;
    private readonly Mock<ICurrentUser> _mockCurrentUser;

    public CreateCartWithFirstProductCommandHandlerTests()
    {
        _mockProductRepository = new Mock<IProductRepository>();
        _mockCartRepository = new Mock<ICartRepository>();
        _mockCurrentUser = new Mock<ICurrentUser>();
        _sut = new CreateCartWithFirstProductCommandHandler(
            _mockProductRepository.Object,
            _mockCartRepository.Object,
            _mockCurrentUser.Object
        );
    }

    // Should returns product not found when product doest not exist
    [Fact]
    public async Task ShouldReturnsProductNotFoundWhenProductDoesNotExist()
    {
        // Arrange
        var command = CreateCartWithFirstProductCommandUtils.CreateCommand();
        _mockProductRepository
            .Setup(x => x.IsProductExistAsync(It.IsAny<ProductId>()))
            .ReturnsAsync(false);

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        _mockProductRepository.Verify(
            x => x.IsProductExistAsync(ProductId.Create(command.ProductId)),
            Times.Once
        );
        _mockCartRepository.Verify(x => x.AddAsync(It.IsAny<Cart>()), Times.Never);
        _mockCartRepository.Verify(x => x.Commit(It.IsAny<CancellationToken>()), Times.Never);

        result.IsError.Should().BeTrue();
        result.Errors.Should().NotBeNullOrEmpty();
        result.Errors[0].Should().Be(CustomErrors.Product.ProductNotFound);
    }

    // Should creates cart with line item and returns when product exists
    [Fact]
    public async Task ShouldCreatesCartWithLineItemAndReturnsWhenProductExists()
    {
        // Arrange
        var cart = CartFactory.Create();
        cart.AddLineItem(CartFactory.CreateCartItem());
        var command = CreateCartWithFirstProductCommandUtils.CreateCommand();
        _mockProductRepository
            .Setup(x => x.IsProductExistAsync(ProductId.Create(command.ProductId)))
            .ReturnsAsync(true);

        _mockCurrentUser.Setup(cu => cu.GetCurrentUserId()).Returns(Constants.User.Id.Value);

        _mockCartRepository.Setup(x => x.AddAsync(It.IsAny<Cart>()));
        _mockCartRepository.Setup(x => x.Commit(It.IsAny<CancellationToken>())).ReturnsAsync(1);
        _mockCartRepository.Setup(cr => cr.GetCartByUserIdAsync(cart.UserId)).ReturnsAsync(cart);

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        _mockProductRepository.Verify(
            x => x.IsProductExistAsync(ProductId.Create(command.ProductId)),
            Times.Once
        );
        _mockCartRepository.Verify(
            x =>
                x.AddAsync(
                    It.Is<Cart>(c => c.UserId == Constants.User.Id && c.CartItems.Count == 1)
                ),
            Times.Once
        );
        _mockCartRepository.Verify(x => x.Commit(It.IsAny<CancellationToken>()), Times.Once);
        _mockCartRepository.Verify(cr => cr.GetCartByUserIdAsync(cart.UserId), Times.Once);

        result.IsError.Should().BeFalse();
        result.Value.Should().NotBeNull();
        result.Value.Should().BeOfType(typeof(Cart));
        result.Value.UserId.Should().Be(Constants.User.Id);
        result.Value.CartItems.Should().HaveCount(1);
        result.Value.CartItems[0].ProductId.Should().Be(ProductId.Create(command.ProductId));
    }
}
