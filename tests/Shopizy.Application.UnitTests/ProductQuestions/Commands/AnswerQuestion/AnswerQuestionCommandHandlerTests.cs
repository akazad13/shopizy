using ErrorOr;
using Moq;
using Shopizy.Application.Common.Interfaces.Persistence;
using Shopizy.Application.ProductQuestions.Commands.AnswerQuestion;
using Shopizy.Domain.Common.CustomErrors;
using Shopizy.Domain.ProductQuestions;
using Shopizy.Domain.ProductQuestions.ValueObjects;
using Shopizy.Domain.Products.ValueObjects;
using Shopizy.Domain.Users.ValueObjects;

namespace Shopizy.Application.UnitTests.ProductQuestions.Commands.AnswerQuestion;

public class AnswerQuestionCommandHandlerTests
{
    private readonly Mock<IProductQuestionRepository> _mockRepository;
    private readonly AnswerQuestionCommandHandler _sut;

    public AnswerQuestionCommandHandlerTests()
    {
        _mockRepository = new Mock<IProductQuestionRepository>();
        _sut = new AnswerQuestionCommandHandler(_mockRepository.Object);
    }

    [Fact]
    public async Task Should_ReturnQuestionNotFound_WhenQuestionDoesNotExist()
    {
        // Arrange
        var command = new AnswerQuestionCommand(Guid.NewGuid(), Guid.NewGuid(), "Sample answer");

        _mockRepository
            .Setup(x => x.GetByIdAsync(It.IsAny<ProductQuestionId>()))
            .ReturnsAsync((ProductQuestion?)null);

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsError);
        Assert.Equal(CustomErrors.ProductQuestion.QuestionNotFound, result.FirstError);
    }

    [Fact]
    public async Task Should_AddAnswer_WhenQuestionExists()
    {
        // Arrange
        var question = ProductQuestion.Create(
            ProductId.CreateUnique(),
            UserId.CreateUnique(),
            "Is this available?"
        );
        var command = new AnswerQuestionCommand(
            question.Id.Value,
            Guid.NewGuid(),
            "Yes, in stock!"
        );

        _mockRepository.Setup(x => x.GetByIdAsync(question.Id)).ReturnsAsync(question);

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(result.IsError);
        Assert.NotNull(result.Value.Answer);
        Assert.Equal("Yes, in stock!", result.Value.Answer.Answer);
    }
}
