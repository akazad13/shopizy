using ErrorOr;
using Shopizy.Application.Common.Security.Permissions;
using Shopizy.Application.Common.Security.Policies;
using Shopizy.Application.Common.Security.Request;

namespace Shopizy.Application.Categories.Commands.DeleteCategory;

[Authorize(Permissions = Permission.Category.Delete, Policies = Policy.SelfOrAdmin)]
public record DeleteCategoryCommand(Guid UserId, Guid CategoryId)
    : IAuthorizeableRequest<ErrorOr<Success>>;
