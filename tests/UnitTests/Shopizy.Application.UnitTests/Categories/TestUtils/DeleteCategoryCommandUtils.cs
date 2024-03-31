using Shopizy.Application.UnitTests.TestUtils.Constants;
using Shopizy.Application.Categories.Commands.DeleteCategory;

namespace Shopizy.Application.UnitTests.Categories.TestUtils;

public static class DeleteCategoryCommandUtils
{
    public static DeleteCategoryCommand CreateCommand()
    {
        return new DeleteCategoryCommand(Constants.User.Id.Value, Constants.Category.Id.Value);
    }
}
