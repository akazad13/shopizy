using ErrorOr;
using Moq;
using Shopizy.Application.Common.Interfaces.Persistence;
using Shopizy.Application.GiftCards.Queries.GetGiftCards;
using Shopizy.Domain.GiftCards;
using Shouldly;

namespace Shopizy.Application.UnitTests.GiftCards.Queries.GetGiftCards;

public class GetGiftCardsQueryHandlerTests
{
    private readonly Mock<IGiftCardRepository> _mockRepository;
    private readonly GetGiftCardsQueryHandler _handler;

    public GetGiftCardsQueryHandlerTests()
    {
        _mockRepository = new Mock<IGiftCardRepository>();
        _handler = new GetGiftCardsQueryHandler(_mockRepository.Object);
    }

    [Fact]
    public async Task Handle_ShouldReturnListOfGiftCards()
    {
        // Arrange
        var query = new GetGiftCardsQuery(1, 10);
        var giftCards = new List<GiftCard>
        {
            GiftCard.Create("CARD1", 50m, null),
            GiftCard.Create("CARD2", 100m, null),
        };

        _mockRepository.Setup(r => r.GetAllAsync(1, 10)).ReturnsAsync(giftCards);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsError.ShouldBeFalse();
        result.Value.Count.ShouldBe(2);
        result.Value[0].Code.ShouldBe("CARD1");
        result.Value[1].Code.ShouldBe("CARD2");
        query.CacheKey.ShouldBe("gift-cards-1-10");
        query.Expiration.ShouldNotBeNull();
    }
}
