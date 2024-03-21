using FluentAssertions;
using Moq;
using Shopizy.Application.Categories.Commands.CreateCategory;
using Shopizy.Application.Common.Interfaces.Persistance;
using Shopizy.Application.UnitTests.Categories.Commands.TestUtils;
using Shopizy.Application.UnitTests.TestUtils.Categories.Extensions;

namespace Shopizy.Application.UnitTests.Categories.Commands.CreateCategory;

public class CreateCategoryCommandHandlerTest
{
    private readonly CreateCategoryCommandHandler _handler;
    private readonly Mock<ICategoryRepository> _mockCategoryRepository;

    public CreateCategoryCommandHandlerTest()
    {
        _mockCategoryRepository = new Mock<ICategoryRepository>();
        _handler = new CreateCategoryCommandHandler(_mockCategoryRepository.Object);
    }

    [Fact]
    public async void CreateCategoryCommand_WhenCategoryIsValid_ShouldCrateAndReturnCategory()
    {
        var createCategoryCommand = CreateCategoryCommandUtils.CreateCommand();

        _mockCategoryRepository.Setup(c => c.Commit(default)).ReturnsAsync(1);

        var result = await _handler.Handle(createCategoryCommand, default);

        result.IsError.Should().BeFalse();
        result.Value.ValidateCreatedForm(createCategoryCommand);
    }
}
