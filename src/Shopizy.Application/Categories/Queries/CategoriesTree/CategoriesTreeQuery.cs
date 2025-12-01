using ErrorOr;
using MediatR;
using Shopizy.Application.Categories.Queries.CategoriesTree;

namespace Shopizy.Application.Categories.Queries.CategoriesTree;

/// <summary>
/// Represents a query to retrieve categories in a hierarchical tree structure.
/// </summary>
public record CategoriesTreeQuery() : IRequest<ErrorOr<List<CategoryTree>>>;
