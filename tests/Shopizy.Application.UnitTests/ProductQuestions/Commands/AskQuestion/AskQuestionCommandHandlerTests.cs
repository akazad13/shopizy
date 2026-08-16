using ErrorOr;
using Moq;
using Shopizy.Application.Common.Interfaces.Persistence;
using Shopizy.Application.ProductQuestions.Commands.AskQuestion;
using Shopizy.Application.UnitTests.Products.TestUtils;
using Shopizy.Domain.Common.CustomErrors;
using Shopizy.Domain.ProductQuestions;
using Shopizy.Domain.Products;
using Shopizy.Domain.Products.ValueObjects;

namespace Shopizy.Application.UnitTests.ProductQuestions.Commands.AskQuestion;

public class AskQuestionCommandHandlerTests
{
    private readonly Mock<IProductQuestionRepository> _mockQuestionRepo;
    private readonly Mock<IProductRepository> _mockProductRepo;
    private readonly AskQuestionCommandHandler _sut;

    public AskQuestionCommandHandlerTests()
    {
        _mockQuestionRepo = new Mock<IProductQuestionRepository>();
        _mockProductRepo = new Mock<IProductRepository>();
        _sut = new AskQuestionCommandHandler(_mockQuestionRepo.Object, _mockProductRepo.Object);
    }

    [Fact]
    public async Task Should_ReturnProductNotFound_WhenProductDoesNotExist()
    {
        // Arrange
        var command = new AskQuestionCommand(
            Guid.NewGuid(),
            Guid.NewGuid(),
            "What is the warranty?"
        );

        _mockProductRepo
            .Setup(x => x.GetProductByIdAsync(It.IsAny<ProductId>()))
            .ReturnsAsync((Product?)null);

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsError);
        Assert.Equal(CustomErrors.Product.ProductNotFound, result.FirstError);
    }

    [Fact]
    public async Task Should_CreateQuestion_WhenProductExists()
    {
        // Arrange
        var product = ProductFactory.CreateProduct();
        var command = new AskQuestionCommand(
            Guid.NewGuid(),
            product.Id.Value,
            "Does this come with warranty?"
        );

        _mockProductRepo.Setup(x => x.GetProductByIdAsync(product.Id)).ReturnsAsync(product);

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(result.IsError);
        Assert.NotNull(result.Value);
        Assert.Equal("Does this come with warranty?", result.Value.Question);

        _mockQuestionRepo.Verify(x => x.AddAsync(It.IsAny<ProductQuestion>()), Times.Once);
    }
}
