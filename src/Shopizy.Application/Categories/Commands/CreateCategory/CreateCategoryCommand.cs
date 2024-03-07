using ErrorOr;
using shopizy.Application.Common.Security.Permissions;
using shopizy.Application.Common.Security.Policies;
using shopizy.Application.Common.Security.Request;
using Shopizy.Domain.Categories;

namespace shopizy.Application.Categories.Commands.CreateCategory;

[Authorize(Permissions = Permission.Category.Create, Policies = Policy.SelfOrAdmin)]
public record CreateCategoryCommand(Guid UserId, string Name, Guid? ParentId)
    : IAuthorizeableRequest<ErrorOr<Category>>;
