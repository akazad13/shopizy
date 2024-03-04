using Mapster;
using shopizy.Application.Categories.Commands.CreateCategory;
using shopizy.Contracts.Category;
using Shopizy.Domain.Categories;

namespace shopizy.Api.Common.Mapping;

public class CategoryMappingConfig : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<CreateCategoryRequest, CreateCategoryCommand>();
        config.NewConfig<Category, CategoryResponse>().Map(dest => dest.Id, src => src.Id.Value);
    }
}
