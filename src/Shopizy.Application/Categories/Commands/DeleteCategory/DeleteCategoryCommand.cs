using ErrorOr;
using Shopizy.Application.Common.Security.Permissions;
using Shopizy.Application.Common.Security.Policies;
using Shopizy.Application.Common.Security.Request;

namespace Shopizy.Application.Categories.Commands.DeleteCategory;

/// <summary>
/// Represents a command to delete a category.
/// </summary>
/// <param name="UserId">The user's unique identifier.</param>
/// <param name="CategoryId">The category's unique identifier to delete.</param>
[Authorize(Permissions = Permissions.Category.Delete, Policies = Policy.Admin)]
public record DeleteCategoryCommand(Guid UserId, Guid CategoryId) : IAuthorizeableRequest<ErrorOr<Success>>;
