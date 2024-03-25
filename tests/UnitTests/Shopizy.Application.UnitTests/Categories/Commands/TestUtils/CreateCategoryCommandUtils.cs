using Shopizy.Application.UnitTests.TestUtils.Constants;
using Shopizy.Application.Categories.Commands.CreateCategory;

namespace Shopizy.Application.UnitTests.Categories.Commands.TestUtils;

public static class CreateCategoryCommandUtils
{
    public static CreateCategoryCommand CreateCommand()
    {
        return new CreateCategoryCommand(
            Constants.User.Id.Value,
            Constants.Category.Name,
            Constants.Category.ParentId
        );
    }
}
