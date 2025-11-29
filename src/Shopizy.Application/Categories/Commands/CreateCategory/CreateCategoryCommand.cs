using ErrorOr;
using Shopizy.Application.Common.Security.Permissions;
using Shopizy.Application.Common.Security.Policies;
using Shopizy.Application.Common.Security.Request;
using Shopizy.Domain.Categories;

namespace Shopizy.Application.Categories.Commands.CreateCategory;

/// <summary>
/// Represents a command to create a new category.
/// </summary>
/// <param name="UserId">The user's unique identifier.</param>
/// <param name="Name">The category name.</param>
/// <param name="ParentId">The parent category ID (null for root categories).</param>
[Authorize(Permissions = Permissions.Category.Create, Policies = Policy.Admin)]
public record CreateCategoryCommand(Guid UserId, string Name, Guid? ParentId)
    : IAuthorizeableRequest<ErrorOr<Category>>;
