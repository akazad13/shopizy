using Shopizy.Infrastructure.Common.Specifications;
using Shopizy.Domain.Categories;
using Shopizy.Domain.Categories.ValueObjects;

namespace Shopizy.Infrastructure.Categories.Specifications;

internal class CategoryByIdSpec(CategoryId id) : Specification<Category>(category => category.Id == id)
{
}