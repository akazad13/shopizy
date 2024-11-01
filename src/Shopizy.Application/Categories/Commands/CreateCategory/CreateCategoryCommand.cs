using Shopizy.Application.Common.Security.Permissions;
using Shopizy.Application.Common.Security.Policies;
using Shopizy.Application.Common.Security.Request;
using Shopizy.Application.Common.Wrappers;
using Shopizy.Domain.Categories;

namespace Shopizy.Application.Categories.Commands.CreateCategory;

[Authorize(Permissions = Permissions.Category.Create, Policies = Policy.SelfOrAdmin)]
public record CreateCategoryCommand(Guid UserId, string Name, Guid? ParentId)
    : IAuthorizeableRequest<IResult<Category>>;
