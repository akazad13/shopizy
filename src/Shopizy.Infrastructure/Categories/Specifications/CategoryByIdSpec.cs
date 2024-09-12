using Shopizy.Domain.Categories;
using Shopizy.Domain.Categories.ValueObjects;
using Shopizy.Infrastructure.Common.Specifications;

namespace Shopizy.Infrastructure.Categories.Specifications;

internal class CategoryByIdSpec(CategoryId id) : Specification<Category>(category => category.Id == id)
{
}
