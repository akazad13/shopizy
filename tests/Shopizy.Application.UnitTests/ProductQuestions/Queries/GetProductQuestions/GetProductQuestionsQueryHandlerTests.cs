using ErrorOr;
using Moq;
using Shopizy.Application.Common.Interfaces.Persistence;
using Shopizy.Application.ProductQuestions.Queries.GetProductQuestions;
using Shopizy.Domain.ProductQuestions;
using Shopizy.Domain.Products.ValueObjects;

namespace Shopizy.Application.UnitTests.ProductQuestions.Queries.GetProductQuestions;

public class GetProductQuestionsQueryHandlerTests
{
    private readonly Mock<IProductQuestionRepository> _mockRepo;
    private readonly GetProductQuestionsQueryHandler _sut;

    public GetProductQuestionsQueryHandlerTests()
    {
        _mockRepo = new Mock<IProductQuestionRepository>();
        _sut = new GetProductQuestionsQueryHandler(_mockRepo.Object);
    }

    [Fact]
    public async Task Should_ReturnQuestionsList_WhenQueryIsHandled()
    {
        // Arrange
        var query = new GetProductQuestionsQuery(Guid.NewGuid(), 1, 10);

        _mockRepo
            .Setup(x =>
                x.GetByProductIdAsync(It.IsAny<ProductId>(), query.PageNumber, query.PageSize)
            )
            .ReturnsAsync([]);

        // Act
        var result = await _sut.Handle(query, CancellationToken.None);

        // Assert
        Assert.False(result.IsError);
        Assert.Empty(result.Value);
    }
}
