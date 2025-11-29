using ErrorOr;
using MediatR;
using Shopizy.Domain.Categories;

namespace Shopizy.Application.Categories.Queries.GetCategory;

/// <summary>
/// Represents a query to retrieve a category by its ID.
/// </summary>
/// <param name="CategoryId">The category's unique identifier.</param>
public record GetCategoryQuery(Guid CategoryId) : IRequest<ErrorOr<Category>>;
