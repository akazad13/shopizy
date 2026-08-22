using Moq;
using Shopizy.Application.Common.Interfaces.Persistence;
using Shopizy.Application.GiftCards.Queries.GetGiftCardById;
using Shopizy.Domain.Common.CustomErrors;
using Shopizy.Domain.GiftCards;
using Shopizy.Domain.GiftCards.ValueObjects;
using Shouldly;
using Xunit;

namespace Shopizy.Application.UnitTests.GiftCards.Queries;

public class GetGiftCardByIdQueryHandlerTests
{
    private readonly Mock<IGiftCardRepository> _mockGiftCardRepository;
    private readonly GetGiftCardByIdQueryHandler _handler;

    public GetGiftCardByIdQueryHandlerTests()
    {
        _mockGiftCardRepository = new Mock<IGiftCardRepository>();
        _handler = new GetGiftCardByIdQueryHandler(_mockGiftCardRepository.Object);
    }

    [Fact]
    public async Task Handle_WhenNotFound_ShouldReturnNotFoundError()
    {
        var id = GiftCardId.CreateUnique();
        _mockGiftCardRepository.Setup(r => r.GetByIdAsync(id)).ReturnsAsync((GiftCard?)null);

        var result = await _handler.Handle(new GetGiftCardByIdQuery(id), CancellationToken.None);

        result.IsError.ShouldBeTrue();
        result.FirstError.Code.ShouldBe(CustomErrors.GiftCard.GiftCardNotFound.Code);
    }

    [Fact]
    public async Task Handle_WhenFound_ShouldReturnGiftCard()
    {
        var card = GiftCard.Create("TEST-123", 50, DateTime.UtcNow.AddDays(30));
        _mockGiftCardRepository.Setup(r => r.GetByIdAsync(card.Id)).ReturnsAsync(card);

        var result = await _handler.Handle(
            new GetGiftCardByIdQuery(card.Id),
            CancellationToken.None
        );

        result.IsError.ShouldBeFalse();
        result.Value.ShouldBe(card);
    }
}
