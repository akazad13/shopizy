using FluentAssertions;
using Moq;
using Shopizy.Application.Carts.Commands.CreateCartWithFirstProduct;
using Shopizy.Application.Common.Interfaces.Persistence;
using Shopizy.Application.UnitTests.Carts.TestUtils;
using Shopizy.Domain.Carts;
using Shopizy.Domain.Common.CustomErrors;
using Shopizy.Domain.Products.ValueObjects;
using Shopizy.Domain.Users.ValueObjects;

namespace Shopizy.Application.UnitTests.Carts.Commands.CreateCartWithFirstProduct;

public class CreateCartWithFirstProductCommandHandlerTests
{
    private readonly CreateCartWithFirstProductCommandHandler _sut;
    private readonly Mock<IProductRepository> _mockProductRepository;
    private readonly Mock<ICartRepository> _mockCartRepository;

    public CreateCartWithFirstProductCommandHandlerTests()
    {
        _mockProductRepository = new Mock<IProductRepository>();
        _mockCartRepository = new Mock<ICartRepository>();
        _sut = new CreateCartWithFirstProductCommandHandler(
            _mockProductRepository.Object,
            _mockCartRepository.Object
        );
    }

    // Should returns product not found when product doest not exist
    [Fact]
    public async Task ShouldReturnsProductNotFoundWhenProductDoesNotExistAsync()
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
    public async Task ShouldCreatesCartWithLineItemAndReturnsWhenProductExistsAsync()
    {
        // Arrange
        var cart = CartFactory.Create();
        cart.AddLineItem(CartFactory.CreateLineItem());
        var command = CreateCartWithFirstProductCommandUtils.CreateCommand();
        _mockProductRepository
            .Setup(x => x.IsProductExistAsync(ProductId.Create(command.ProductId)))
            .ReturnsAsync(true);

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
                    It.Is<Cart>(c => c.UserId.Value == command.UserId && c.LineItems.Count == 1)
                ),
            Times.Once
        );
        _mockCartRepository.Verify(x => x.Commit(It.IsAny<CancellationToken>()), Times.Once);
        _mockCartRepository.Verify(cr => cr.GetCartByUserIdAsync(cart.UserId), Times.Once);

        result.IsError.Should().BeFalse();
        result.Value.Should().NotBeNull();
        result.Value.Should().BeOfType(typeof(Cart));
        result.Value.UserId.Should().Be(UserId.Create(command.UserId));
        result.Value.LineItems.Should().HaveCount(1);
        result.Value.LineItems[0].ProductId.Should().Be(ProductId.Create(command.ProductId));
    }
}
