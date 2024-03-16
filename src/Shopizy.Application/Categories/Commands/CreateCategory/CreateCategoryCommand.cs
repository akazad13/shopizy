using ErrorOr;
using Shopizy.Application.Common.Security.Permissions;
using Shopizy.Application.Common.Security.Policies;
using Shopizy.Application.Common.Security.Request;
using Shopizy.Domain.Categories;
using Shopizy.Domain.Users.ValueObjects;

namespace Shopizy.Application.Categories.Commands.CreateCategory;

[Authorize(Permissions = Permission.Category.Create, Policies = Policy.SelfOrAdmin)]
public record CreateCategoryCommand(UserId UserId, string Name, Guid? ParentId)
    : IAuthorizeableRequest<ErrorOr<Category>>;
