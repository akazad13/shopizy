using ErrorOr;
using Moq;
using Shopizy.Application.Common.Interfaces.Persistence;
using Shopizy.Application.PromoCodes.Commands.CreatePromoCode;
using Shopizy.Domain.Common.CustomErrors;
using Shopizy.Domain.PromoCodes;

namespace Shopizy.Application.UnitTests.PromoCodes.Commands.CreatePromoCode;

public class CreatePromoCodeCommandHandlerTests
{
    private readonly Mock<IPromoCodeRepository> _mockRepo;
    private readonly CreatePromoCodeCommandHandler _sut;

    public CreatePromoCodeCommandHandlerTests()
    {
        _mockRepo = new Mock<IPromoCodeRepository>();
        _sut = new CreatePromoCodeCommandHandler(_mockRepo.Object);
    }

    [Fact]
    public async Task Should_ReturnDuplicateCodeError_WhenCodeAlreadyExists()
    {
        // Arrange
        var command = new CreatePromoCodeCommand("SUMMER20", "20% off", 20, true, true);
        var existingPromo = PromoCode.Create("SUMMER20", "Existing", 20, true, true);

        _mockRepo.Setup(x => x.GetByCodeAsync("SUMMER20")).ReturnsAsync(existingPromo);

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsError);
        Assert.Equal(CustomErrors.PromoCode.DuplicateCode, result.FirstError);
    }

    [Fact]
    public async Task Should_CreatePromoCode_WhenCodeIsUnique()
    {
        // Arrange
        var command = new CreatePromoCodeCommand("SUMMER20", "20% off", 20, true, true);

        _mockRepo.Setup(x => x.GetByCodeAsync("SUMMER20")).ReturnsAsync((PromoCode?)null);

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(result.IsError);
        Assert.Equal("SUMMER20", result.Value.Code);
        _mockRepo.Verify(x => x.AddAsync(It.IsAny<PromoCode>()), Times.Once);
    }
}
