using ErrorOr;
using MediatR;
using Shopizy.Domain.Categories;

namespace shopizy.Application.Categories.Queries.ListCategories;

public record ListCategoriesQuery() : IRequest<ErrorOr<List<Category>>>;
