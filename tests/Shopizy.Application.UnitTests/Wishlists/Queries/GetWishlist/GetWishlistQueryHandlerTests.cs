using ErrorOr;
using Moq;
using Shopizy.Application.Common.Interfaces.Persistence;
using Shopizy.Application.Wishlists.Queries.GetWishlist;
using Shopizy.Domain.Common.CustomErrors;
using Shopizy.Domain.Users.ValueObjects;
using Shopizy.Domain.Wishlists;
using Shouldly;

namespace Shopizy.Application.UnitTests.Wishlists.Queries.GetWishlist;

public class GetWishlistQueryHandlerTests
{
    private readonly Mock<IWishlistRepository> _mockWishlistRepository;
    private readonly GetWishlistQueryHandler _handler;

    public GetWishlistQueryHandlerTests()
    {
        _mockWishlistRepository = new Mock<IWishlistRepository>();
        _handler = new GetWishlistQueryHandler(_mockWishlistRepository.Object);
    }

    [Fact]
    public async Task Handle_WhenWishlistNotFound_ShouldReturnWishlistNotFound()
    {
        // Arrange
        _mockWishlistRepository
            .Setup(r =>
                r.GetWishlistByUserIdAsync(It.IsAny<UserId>(), It.IsAny<CancellationToken>())
            )
            .ReturnsAsync((Wishlist?)null);

        var query = new GetWishlistQuery(Guid.NewGuid());

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsError.ShouldBeTrue();
        result.FirstError.Code.ShouldBe(CustomErrors.Wishlist.WishlistNotFound.Code);
    }

    [Fact]
    public async Task Handle_WhenWishlistFound_ShouldReturnWishlist()
    {
        // Arrange
        var userId = UserId.Create(Guid.NewGuid());
        var wishlist = Wishlist.Create(userId, "My Favorites", isPublic: true);

        _mockWishlistRepository
            .Setup(r =>
                r.GetWishlistByUserIdAsync(
                    It.Is<UserId>(id => id.Value == userId.Value),
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync(wishlist);

        var query = new GetWishlistQuery(userId.Value);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsError.ShouldBeFalse();
        result.Value.Name.ShouldBe("My Favorites");
        result.Value.IsPublic.ShouldBeTrue();
    }
}
