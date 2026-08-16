using ErrorOr;
using Moq;
using Shopizy.Application.Common.Interfaces.Persistence;
using Shopizy.Application.Wishlists.Commands.DeleteWishlist;
using Shopizy.Domain.Common.CustomErrors;
using Shopizy.Domain.Users.ValueObjects;
using Shopizy.Domain.Wishlists;
using Shouldly;
using Xunit;

namespace Shopizy.Application.UnitTests.Wishlists.Commands;

public class DeleteWishlistCommandHandlerTests
{
    private readonly Mock<IWishlistRepository> _mockWishlistRepository;
    private readonly DeleteWishlistCommandHandler _sut;

    public DeleteWishlistCommandHandlerTests()
    {
        _mockWishlistRepository = new Mock<IWishlistRepository>();
        _sut = new DeleteWishlistCommandHandler(_mockWishlistRepository.Object);
    }

    [Fact]
    public async Task Handle_WhenWishlistNotFound_ReturnsNotFound()
    {
        // Arrange
        var userId = UserId.CreateUnique();
        var command = new DeleteWishlistCommand(userId);

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
    public async Task Handle_WhenWishlistExists_RemovesWishlistFromRepository()
    {
        // Arrange
        var userId = UserId.CreateUnique();
        var wishlist = Wishlist.Create(userId, "My Favorites", false);
        var command = new DeleteWishlistCommand(userId);

        _mockWishlistRepository
            .Setup(x => x.GetWishlistByUserIdAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(wishlist);

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.IsError.ShouldBeFalse();
        result.Value.ShouldBe(Result.Success);
        _mockWishlistRepository.Verify(x => x.Remove(wishlist), Times.Once);
    }
}
