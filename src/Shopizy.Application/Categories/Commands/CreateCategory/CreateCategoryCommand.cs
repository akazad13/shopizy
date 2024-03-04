using ErrorOr;
using MediatR;
using Shopizy.Domain.Categories;

namespace shopizy.Application.Categories.Commands.CreateCategory;

public record CreateCategoryCommand(string Name, Guid? ParentId) : IRequest<ErrorOr<Category>>;
