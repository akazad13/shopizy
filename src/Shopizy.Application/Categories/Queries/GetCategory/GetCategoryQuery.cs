using ErrorOr;
using MediatR;
using Shopizy.Domain.Categories;

namespace shopizy.Application.Categories.Queries.GetCategory;

public record GetCategoryQuery(Guid CategoryId) : IRequest<ErrorOr<Category?>>;
