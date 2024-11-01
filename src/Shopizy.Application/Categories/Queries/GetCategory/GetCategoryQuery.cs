using MediatR;
using Shopizy.Application.Common.Wrappers;
using Shopizy.Domain.Categories;

namespace Shopizy.Application.Categories.Queries.GetCategory;

public record GetCategoryQuery(Guid CategoryId) : IRequest<IResult<Category?>>;
