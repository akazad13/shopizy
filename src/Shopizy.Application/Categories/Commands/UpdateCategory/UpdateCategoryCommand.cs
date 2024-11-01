using Shopizy.Application.Common.Security.Permissions;
using Shopizy.Application.Common.Security.Policies;
using Shopizy.Application.Common.Security.Request;
using Shopizy.Application.Common.Wrappers;
using Shopizy.Domain.Categories;

namespace Shopizy.Application.Categories.Commands.UpdateCategory;

[Authorize(Permissions = Permissions.Category.Modify, Policies = Policy.SelfOrAdmin)]
public record UpdateCategoryCommand(Guid UserId, Guid CategoryId, string Name, Guid? ParentId)
    : IAuthorizeableRequest<IResult<Category>>;
