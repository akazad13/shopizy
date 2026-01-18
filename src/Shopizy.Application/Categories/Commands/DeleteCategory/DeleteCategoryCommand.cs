using ErrorOr;

namespace Shopizy.Application.Categories.Commands.DeleteCategory;

/// <summary>
/// Represents a command to delete a category.
/// </summary>
/// <param name="UserId">The user's unique identifier.</param>
/// <param name="CategoryId">The category's unique identifier to delete.</param>
public record DeleteCategoryCommand(Guid UserId, Guid CategoryId) : MediatR.IRequest<ErrorOr<Success>>;
