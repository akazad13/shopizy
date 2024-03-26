using Shopizy.Application.UnitTests.TestUtils.Constants;
using Shopizy.Application.Categories.Commands.UpdateCategory;

namespace Shopizy.Application.UnitTests.Categories.Commands.TestUtils;

public static class UpdateCategoryCommandUtils
{
    public static UpdateCategoryCommand CreateCommand()
    {
        return new UpdateCategoryCommand(
            Constants.User.Id.Value,
            Constants.Category.Id.Value,
            Constants.Category.Name,
            Constants.Category.ParentId
        );
    }
}
