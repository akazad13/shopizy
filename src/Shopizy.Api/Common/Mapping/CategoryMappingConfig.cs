using Mapster;
using shopizy.Application.Categories.Commands.CreateCategory;
using shopizy.Application.Categories.Queries.GetCategory;
using shopizy.Application.Categories.Queries.ListCategoriesQuery;
using shopizy.Contracts.Category;
using Shopizy.Domain.Categories;

namespace shopizy.Api.Common.Mapping;

public class CategoryMappingConfig : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config
            .NewConfig<(Guid UserId, CreateCategoryRequest Request), CreateCategoryCommand>()
            .Map(dest => dest.UserId, src => src.UserId)
            .Map(dest => dest, src => src.Request);

        config.NewConfig<Category, CategoryResponse>().Map(dest => dest.Id, src => src.Id.Value);

        config
            .NewConfig<(Guid UserId, Guid CategoryId), GetCategoryQuery>()
            .Map(dest => dest.UserId, src => src.UserId)
            .Map(dest => dest.CategoryId, src => src.CategoryId);
    }
}
