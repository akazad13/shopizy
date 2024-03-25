using ErrorOr;
using MediatR;
using Shopizy.Domain.Categories;

namespace Shopizy.Application.Categories.Queries.GetCategory;

public record GetCategoryQuery(Guid CategoryId) : IRequest<ErrorOr<Category?>>;
