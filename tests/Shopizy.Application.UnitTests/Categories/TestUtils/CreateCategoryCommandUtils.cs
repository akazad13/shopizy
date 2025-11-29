using Shopizy.Application.Categories.Commands.CreateCategory;
using Shopizy.Application.UnitTests.TestUtils.Constants;

namespace Shopizy.Application.UnitTests.Categories.TestUtils;

public static class CreateCategoryCommandUtils
{
    public static CreateCategoryCommand CreateCommand()
    {
        return new CreateCategoryCommand(Constants.User.Id.Value, Constants.Category.Name, Constants.Category.ParentId);
    }
}
