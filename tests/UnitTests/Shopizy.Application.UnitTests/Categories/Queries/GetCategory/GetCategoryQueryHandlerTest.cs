using FluentAssertions;
using Moq;
using Shopizy.Application.Categories.Queries.GetCategory;
using Shopizy.Application.Common.Interfaces.Persistance;
using Shopizy.Application.UnitTests.Categories.Queries.TestUtils;
using Shopizy.Application.UnitTests.TestUtils.Categories.Extensions;
using Shopizy.Application.UnitTests.TestUtils.Constants;
using Shopizy.Domain.Categories;

namespace Shopizy.Application.UnitTests.Categories.Queries.GetCategory;

public class GetCategoryQueryHandlerTest
{
    private readonly GetCategoryQueryHandler _handler;
    private readonly Mock<ICategoryRepository> _mockCategoryRepository;

    public GetCategoryQueryHandlerTest()
    {
        _mockCategoryRepository = new Mock<ICategoryRepository>();
        _handler = new GetCategoryQueryHandler(_mockCategoryRepository.Object);
    }

    [Fact]
    public async void GetCategoryQuery_WhenCategoryIsFound_ShouldReturnCategory()
    {
        var getCategoryQuery = GetCategoryQueryUtils.CreateQuery();
        _mockCategoryRepository
            .Setup(c => c.GetCategoryByIdAsync(getCategoryQuery.CategoryId))
            .ReturnsAsync(Category.Create(Constants.Category.Name, Constants.Category.ParentId));

        var result = await _handler.Handle(getCategoryQuery, default);

        result.IsError.Should().BeFalse();
        result.Value?.ValidateCreatedForm(getCategoryQuery);
    }
}
