using ErrorOr;
using Shopizy.SharedKernel.Application.Messaging;
using Shopizy.Domain.Categories;

namespace Shopizy.Application.Categories.Queries.GetCategory;

/// <summary>
/// Represents a query to retrieve a category by its ID.
/// </summary>
/// <param name="CategoryId">The category's unique identifier.</param>
public record GetCategoryQuery(Guid CategoryId) : IQuery<ErrorOr<Category>>;
