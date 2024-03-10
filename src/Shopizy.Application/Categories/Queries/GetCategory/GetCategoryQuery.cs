using ErrorOr;
using shopizy.Application.Common.Security.Permissions;
using shopizy.Application.Common.Security.Policies;
using shopizy.Application.Common.Security.Request;
using Shopizy.Domain.Categories;

namespace shopizy.Application.Categories.Queries.GetCategory;

[Authorize(Permissions = Permission.Category.Get, Policies = Policy.SelfOrAdmin)]
public record GetCategoryQuery(Guid UserId, Guid CategoryId)
    : IAuthorizeableRequest<ErrorOr<Category?>>;
