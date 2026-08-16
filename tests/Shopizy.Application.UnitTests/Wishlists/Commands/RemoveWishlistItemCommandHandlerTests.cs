using ErrorOr;
using Moq;
using Shopizy.Application.Common.Interfaces.Persistence;
using Shopizy.Application.Wishlists.Commands.RemoveWishlistItem;
using Shopizy.Domain.Common.CustomErrors;
using Shopizy.Domain.Products.ValueObjects;
using Shopizy.Domain.Users.ValueObjects;
using Shopizy.Domain.Wishlists;
using Shouldly;
using Xunit;

namespace Shopizy.Application.UnitTests.Wishlists.Commands;

public class RemoveWishlistItemCommandHandlerTests
{
    private readonly Mock<IWishlistRepository> _mockWishlistRepository;
    private readonly RemoveWishlistItemCommandHandler _sut;

    public RemoveWishlistItemCommandHandlerTests()
    {
        _mockWishlistRepository = new Mock<IWishlistRepository>();
        _sut = new RemoveWishlistItemCommandHandler(_mockWishlistRepository.Object);
    }

    [Fact]
    public async Task Handle_WhenWishlistNotFound_ReturnsNotFound()
    {
        // Arrange
        var userId = UserId.CreateUnique();
        var productId = ProductId.CreateUnique();
        var command = new RemoveWishlistItemCommand(userId, productId);

        _mockWishlistRepository
            .Setup(x => x.GetWishlistByUserIdAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Wishlist?)null);

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.IsError.ShouldBeTrue();
        result.FirstError.ShouldBe((Error)CustomErrors.Wishlist.WishlistNotFound);
    }

    [Fact]
    public async Task Handle_WhenWishlistExists_RemovesItemAndUpdateRepository()
    {
        // Arrange
        var userId = UserId.CreateUnique();
        var productId = ProductId.CreateUnique();
        var wishlist = Wishlist.Create(userId, "My Favorites", false);
        wishlist.AddItem(productId);

        var command = new RemoveWishlistItemCommand(userId, productId);

        _mockWishlistRepository
            .Setup(x => x.GetWishlistByUserIdAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(wishlist);

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.IsError.ShouldBeFalse();
        result.Value.WishlistItems.ShouldBeEmpty();
        _mockWishlistRepository.Verify(x => x.Update(wishlist), Times.Once);
    }
}
