using ErrorOr;
using Moq;
using Shopizy.Application.Common.Interfaces.Persistence;
using Shopizy.Application.PromoCodes.Queries.GetPromoCodes;
using Shopizy.Domain.PromoCodes;

namespace Shopizy.Application.UnitTests.PromoCodes.Queries.GetPromoCodes;

public class GetPromoCodesQueryHandlerTests
{
    private readonly Mock<IPromoCodeRepository> _mockRepo;
    private readonly GetPromoCodesQueryHandler _sut;

    public GetPromoCodesQueryHandlerTests()
    {
        _mockRepo = new Mock<IPromoCodeRepository>();
        _sut = new GetPromoCodesQueryHandler(_mockRepo.Object);
    }

    [Fact]
    public async Task Should_ReturnPromoCodesList_WhenQueryIsHandled()
    {
        // Arrange
        var query = new GetPromoCodesQuery(1, 10);
        var promoCode = PromoCode.Create("WELCOME10", "Welcome 10%", 10, true, true);

        _mockRepo.Setup(x => x.GetPromoCodesAsync(1, 10)).ReturnsAsync([promoCode]);

        // Act
        var result = await _sut.Handle(query, CancellationToken.None);

        // Assert
        Assert.False(result.IsError);
        Assert.Single(result.Value);
        Assert.Equal("WELCOME10", result.Value[0].Code);
    }
}
