using ErrorOr;
using Shopizy.Application.Common.Security.Permissions;
using Shopizy.Application.Common.Security.Policies;
using Shopizy.Application.Common.Security.Request;

namespace Shopizy.Application.Categories.Commands.UpdateCategory;

/// <summary>
/// Represents a command to update an existing category.
/// </summary>
/// <param name="UserId">The user's unique identifier.</param>
/// <param name="CategoryId">The category's unique identifier to update.</param>
/// <param name="Name">The new category name.</param>
/// <param name="ParentId">The new parent category ID (null for root categories).</param>
[Authorize(Permissions = Permissions.Category.Modify, Policies = Policy.Admin)]
public record UpdateCategoryCommand(Guid UserId, Guid CategoryId, string Name, Guid? ParentId)
    : IAuthorizeableRequest<ErrorOr<Success>>;
