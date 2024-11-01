using MediatR;
using shopizy.Application.Categories.Queries.CategoriesTree;
using Shopizy.Application.Common.Wrappers;

namespace Shopizy.Application.Categories.Queries.CategoriesTree;

public record CategoriesTreeQuery() : IRequest<IResult<List<CategoryTree>>>;
