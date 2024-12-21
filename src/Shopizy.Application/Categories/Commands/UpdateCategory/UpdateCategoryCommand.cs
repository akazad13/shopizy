using ErrorOr;
using Shopizy.Application.Common.Security.Permissions;
using Shopizy.Application.Common.Security.Policies;
using Shopizy.Application.Common.Security.Request;

namespace Shopizy.Application.Categories.Commands.UpdateCategory;

[Authorize(Permissions = Permissions.Category.Modify, Policies = Policy.Admin)]
public record UpdateCategoryCommand(Guid CategoryId, string Name, Guid? ParentId)
    : IAuthorizeableRequest<ErrorOr<Success>>;
