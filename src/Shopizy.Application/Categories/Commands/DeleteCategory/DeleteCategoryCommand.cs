using Shopizy.Application.Common.Security.Permissions;
using Shopizy.Application.Common.Security.Policies;
using Shopizy.Application.Common.Security.Request;
using Shopizy.Application.Common.Wrappers;

namespace Shopizy.Application.Categories.Commands.DeleteCategory;

[Authorize(Permissions = Permissions.Category.Delete, Policies = Policy.SelfOrAdmin)]
public record DeleteCategoryCommand(Guid UserId, Guid CategoryId)
    : IAuthorizeableRequest<IResult<GenericResponse>>;
