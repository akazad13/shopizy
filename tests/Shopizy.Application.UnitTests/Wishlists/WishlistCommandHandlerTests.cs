using Moq;
using Shopizy.Application.Common.Interfaces.Persistence;
using Shopizy.Application.Wishlists.Commands.UpdateWishlistSettings;
using Shopizy.Application.Wishlists.Queries.GetPublicWishlist;
using Shopizy.Domain.Users.ValueObjects;
using Shopizy.Domain.Wishlists;
using Shopizy.Domain.Wishlists.ValueObjects;
using Shouldly;
using Xunit;

namespace Shopizy.Application.UnitTests.Wishlists;

public class WishlistCommandHandlerTests
{
    private readonly Mock<IWishlistRepository> _mockWishlistRepository = new();

    [Fact]
    public async Task UpdateWishlistSettings_WhenWishlistExists_ShouldUpdateAndReturnWishlist()
    {
        // Arrange
        var userId = UserId.CreateUnique();
        var wishlist = Wishlist.Create(userId, "Old Name", false);
        _mockWishlistRepository
            .Setup(r =>
                r.GetWishlistByUserIdAsync(It.IsAny<UserId>(), It.IsAny<CancellationToken>())
            )
            .ReturnsAsync(wishlist);

        var handler = new UpdateWishlistSettingsCommandHandler(_mockWishlistRepository.Object);
        var command = new UpdateWishlistSettingsCommand(userId.Value, "New Name", true);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsError.ShouldBeFalse();
        result.Value.Name.ShouldBe("New Name");
        result.Value.IsPublic.ShouldBeTrue();
    }

    [Fact]
    public async Task UpdateWishlistSettings_WhenWishlistNotFound_ShouldReturnError()
    {
        // Arrange
        _mockWishlistRepository
            .Setup(r =>
                r.GetWishlistByUserIdAsync(It.IsAny<UserId>(), It.IsAny<CancellationToken>())
            )
            .ReturnsAsync((Wishlist?)null);

        var handler = new UpdateWishlistSettingsCommandHandler(_mockWishlistRepository.Object);
        var command = new UpdateWishlistSettingsCommand(Guid.NewGuid(), "New Name", true);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsError.ShouldBeTrue();
    }

    [Fact]
    public async Task GetPublicWishlist_WhenPublic_ShouldReturnWishlist()
    {
        // Arrange
        var userId = UserId.CreateUnique();
        var wishlist = Wishlist.Create(userId, "Public List", true);
        _mockWishlistRepository
            .Setup(r =>
                r.GetWishlistByIdAsync(It.IsAny<WishlistId>(), It.IsAny<CancellationToken>())
            )
            .ReturnsAsync(wishlist);

        var handler = new GetPublicWishlistQueryHandler(_mockWishlistRepository.Object);
        var query = new GetPublicWishlistQuery(wishlist.Id.Value);

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsError.ShouldBeFalse();
        result.Value.ShouldBe(wishlist);
    }

    [Fact]
    public async Task GetPublicWishlist_WhenPrivate_ShouldReturnForbiddenError()
    {
        // Arrange
        var userId = UserId.CreateUnique();
        var wishlist = Wishlist.Create(userId, "Private List", false);
        _mockWishlistRepository
            .Setup(r =>
                r.GetWishlistByIdAsync(It.IsAny<WishlistId>(), It.IsAny<CancellationToken>())
            )
            .ReturnsAsync(wishlist);

        var handler = new GetPublicWishlistQueryHandler(_mockWishlistRepository.Object);
        var query = new GetPublicWishlistQuery(wishlist.Id.Value);

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsError.ShouldBeTrue();
    }

    [Fact]
    public async Task GetPublicWishlist_WhenNotFound_ShouldReturnNotFoundError()
    {
        // Arrange
        _mockWishlistRepository
            .Setup(r =>
                r.GetWishlistByIdAsync(It.IsAny<WishlistId>(), It.IsAny<CancellationToken>())
            )
            .ReturnsAsync((Wishlist?)null);

        var handler = new GetPublicWishlistQueryHandler(_mockWishlistRepository.Object);
        var query = new GetPublicWishlistQuery(Guid.NewGuid());

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsError.ShouldBeTrue();
    }
}
