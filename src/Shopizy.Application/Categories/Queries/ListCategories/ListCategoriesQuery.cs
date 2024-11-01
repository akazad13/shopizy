using MediatR;
using Shopizy.Application.Common.Wrappers;
using Shopizy.Domain.Categories;

namespace Shopizy.Application.Categories.Queries.ListCategories;

public record ListCategoriesQuery() : IRequest<IResult<List<Category>>>;
