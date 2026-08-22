using Moq;
using Shopizy.Application.Common.Interfaces.Persistence;
using Shopizy.Application.Wishlists.Commands.UpdateWishlist;
using Shopizy.Domain.Common.CustomErrors;
using Shopizy.Domain.Products.ValueObjects;
using Shopizy.Domain.Users.ValueObjects;
using Shopizy.Domain.Wishlists;
using Shouldly;
using Xunit;

namespace Shopizy.Application.UnitTests.Wishlists.Commands.UpdateWishlist;

public class UpdateWishlistCommandHandlerTests
{
    private readonly Mock<IWishlistRepository> _mockWishlistRepository;
    private readonly Mock<IProductRepository> _mockProductRepository;
    private readonly UpdateWishlistCommandHandler _handler;

    public UpdateWishlistCommandHandlerTests()
    {
        _mockWishlistRepository = new Mock<IWishlistRepository>();
        _mockProductRepository = new Mock<IProductRepository>();
        _handler = new UpdateWishlistCommandHandler(
            _mockWishlistRepository.Object,
            _mockProductRepository.Object
        );
    }

    [Fact]
    public async Task Handle_WhenWishlistNotFound_ShouldReturnWishlistNotFound()
    {
        var userId = Guid.NewGuid();
        var productId = Guid.NewGuid();
        _mockWishlistRepository
            .Setup(r =>
                r.GetWishlistByUserIdAsync(It.IsAny<UserId>(), It.IsAny<CancellationToken>())
            )
            .ReturnsAsync((Wishlist?)null);

        var command = new UpdateWishlistCommand(userId, productId, WishlistAction.Add);
        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsError.ShouldBeTrue();
        result.FirstError.Code.ShouldBe(CustomErrors.Wishlist.WishlistNotFound.Code);
    }

    [Fact]
    public async Task Handle_WhenProductNotFound_ShouldReturnProductNotFound()
    {
        var userId = UserId.CreateUnique();
        var wishlist = Wishlist.Create(userId, "My Wishlist", false);
        var productId = Guid.NewGuid();

        _mockWishlistRepository
            .Setup(r =>
                r.GetWishlistByUserIdAsync(It.IsAny<UserId>(), It.IsAny<CancellationToken>())
            )
            .ReturnsAsync(wishlist);
        _mockProductRepository
            .Setup(r => r.IsProductExistAsync(It.IsAny<ProductId>()))
            .ReturnsAsync(false);

        var command = new UpdateWishlistCommand(userId.Value, productId, WishlistAction.Add);
        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsError.ShouldBeTrue();
        result.FirstError.Code.ShouldBe(CustomErrors.Product.ProductNotFound.Code);
    }

    [Fact]
    public async Task Handle_WhenAddActionAndProductAlreadyInWishlist_ShouldReturnError()
    {
        var userId = UserId.CreateUnique();
        var wishlist = Wishlist.Create(userId, "My Wishlist", false);
        var productId = ProductId.CreateUnique();
        wishlist.AddItem(productId);

        _mockWishlistRepository
            .Setup(r =>
                r.GetWishlistByUserIdAsync(It.IsAny<UserId>(), It.IsAny<CancellationToken>())
            )
            .ReturnsAsync(wishlist);
        _mockProductRepository
            .Setup(r => r.IsProductExistAsync(It.IsAny<ProductId>()))
            .ReturnsAsync(true);

        var command = new UpdateWishlistCommand(userId.Value, productId.Value, WishlistAction.Add);
        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsError.ShouldBeTrue();
        result.FirstError.Code.ShouldBe(CustomErrors.Wishlist.ProductAlreadyInWishlist.Code);
    }

    [Fact]
    public async Task Handle_WhenAddActionAndProductNotInWishlist_ShouldAddItem()
    {
        var userId = UserId.CreateUnique();
        var wishlist = Wishlist.Create(userId, "My Wishlist", false);
        var productId = ProductId.CreateUnique();

        _mockWishlistRepository
            .Setup(r =>
                r.GetWishlistByUserIdAsync(It.IsAny<UserId>(), It.IsAny<CancellationToken>())
            )
            .ReturnsAsync(wishlist);
        _mockProductRepository
            .Setup(r => r.IsProductExistAsync(It.IsAny<ProductId>()))
            .ReturnsAsync(true);

        var command = new UpdateWishlistCommand(userId.Value, productId.Value, WishlistAction.Add);
        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsError.ShouldBeFalse();
        result.Value.WishlistItems.ShouldContain(i => i.ProductId == productId);
    }

    [Fact]
    public async Task Handle_WhenRemoveActionAndProductNotInWishlist_ShouldReturnError()
    {
        var userId = UserId.CreateUnique();
        var wishlist = Wishlist.Create(userId, "My Wishlist", false);
        var productId = ProductId.CreateUnique();

        _mockWishlistRepository
            .Setup(r =>
                r.GetWishlistByUserIdAsync(It.IsAny<UserId>(), It.IsAny<CancellationToken>())
            )
            .ReturnsAsync(wishlist);
        _mockProductRepository
            .Setup(r => r.IsProductExistAsync(It.IsAny<ProductId>()))
            .ReturnsAsync(true);

        var command = new UpdateWishlistCommand(
            userId.Value,
            productId.Value,
            WishlistAction.Remove
        );
        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsError.ShouldBeTrue();
        result.FirstError.Code.ShouldBe(CustomErrors.Wishlist.ProductNotInWishlist.Code);
    }

    [Fact]
    public async Task Handle_WhenRemoveActionAndProductInWishlist_ShouldRemoveItem()
    {
        var userId = UserId.CreateUnique();
        var wishlist = Wishlist.Create(userId, "My Wishlist", false);
        var productId = ProductId.CreateUnique();
        wishlist.AddItem(productId);

        _mockWishlistRepository
            .Setup(r =>
                r.GetWishlistByUserIdAsync(It.IsAny<UserId>(), It.IsAny<CancellationToken>())
            )
            .ReturnsAsync(wishlist);
        _mockProductRepository
            .Setup(r => r.IsProductExistAsync(It.IsAny<ProductId>()))
            .ReturnsAsync(true);

        var command = new UpdateWishlistCommand(
            userId.Value,
            productId.Value,
            WishlistAction.Remove
        );
        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsError.ShouldBeFalse();
        result.Value.WishlistItems.ShouldNotContain(i => i.ProductId == productId);
    }
}
