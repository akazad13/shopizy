using ErrorOr;
using MediatR;
using Shopizy.Domain.Categories;

namespace Shopizy.Application.Categories.Queries.ListCategories;

public record ListCategoriesQuery() : IRequest<ErrorOr<List<Category>>>;
