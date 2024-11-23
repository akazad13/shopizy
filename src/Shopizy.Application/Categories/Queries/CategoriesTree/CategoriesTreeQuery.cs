using ErrorOr;
using MediatR;
using shopizy.Application.Categories.Queries.CategoriesTree;

namespace Shopizy.Application.Categories.Queries.CategoriesTree;

public record CategoriesTreeQuery() : IRequest<ErrorOr<List<CategoryTree>>>;
