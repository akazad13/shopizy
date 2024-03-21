using ErrorOr;
using MediatR;
using Shopizy.Domain.Categories;
using Shopizy.Domain.Categories.ValueObjects;

namespace Shopizy.Application.Categories.Queries.GetCategory;

public record GetCategoryQuery(CategoryId CategoryId) : IRequest<ErrorOr<Category?>>;
