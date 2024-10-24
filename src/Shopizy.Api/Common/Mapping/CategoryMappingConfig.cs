using Mapster;
using Shopizy.Application.Categories.Commands.CreateCategory;
using Shopizy.Application.Categories.Commands.DeleteCategory;
using Shopizy.Application.Categories.Commands.UpdateCategory;
using Shopizy.Application.Categories.Queries.GetCategory;
using shopizy.Contracts.Category;
using Shopizy.Contracts.Category;
using Shopizy.Domain.Categories;

namespace Shopizy.Api.Common.Mapping;

public class CategoryMappingConfig : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        _ = config
            .NewConfig<(Guid UserId, CreateCategoryRequest Request), CreateCategoryCommand>()
            .Map(dest => dest.UserId, src => src.UserId)
            .Map(dest => dest, src => src.Request);

        _ = config
            .NewConfig<
                (Guid UserId, Guid CategoryId, UpdateCategoryRequest Request),
                UpdateCategoryCommand
            >()
            .Map(dest => dest.UserId, src => src.UserId)
            .Map(dest => dest.CategoryId, src => src.CategoryId)
            .Map(dest => dest, src => src.Request);

        _ = config
            .NewConfig<(Guid UserId, Guid CategoryId), DeleteCategoryCommand>()
            .Map(dest => dest.UserId, src => src.UserId)
            .Map(dest => dest.CategoryId, src => src.CategoryId);

        _ = config
            .NewConfig<Category, CategoryResponse>()
            .Map(dest => dest.Id, src => src.Id.Value);

        _ = config
            .NewConfig<Category, CategoryTreeResponse>()
            .Map(dest => dest.Id, src => src.Id.Value);

        _ = config.NewConfig<Guid, GetCategoryQuery>().MapWith(src => new GetCategoryQuery(src));
    }
}
