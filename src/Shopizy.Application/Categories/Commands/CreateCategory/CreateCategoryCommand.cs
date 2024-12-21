using ErrorOr;
using Shopizy.Application.Common.Security.Permissions;
using Shopizy.Application.Common.Security.Policies;
using Shopizy.Application.Common.Security.Request;
using Shopizy.Domain.Categories;

namespace Shopizy.Application.Categories.Commands.CreateCategory;

[Authorize(Permissions = Permissions.Category.Create, Policies = Policy.Admin)]
public record CreateCategoryCommand(string Name, Guid? ParentId)
    : IAuthorizeableRequest<ErrorOr<Category>>;
