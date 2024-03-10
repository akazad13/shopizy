using ErrorOr;
using shopizy.Application.Common.Security.Permissions;
using shopizy.Application.Common.Security.Policies;
using shopizy.Application.Common.Security.Request;
using Shopizy.Domain.Categories;

namespace shopizy.Application.Categories.Queries.ListCategoriesQuery;

[Authorize(Permissions = Permission.Category.Get, Policies = Policy.SelfOrAdmin)]
public record ListCategoriesQuery(Guid UserId) : IAuthorizeableRequest<ErrorOr<List<Category>>>;
