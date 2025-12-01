using Ardalis.GuardClauses;
using Mapster;
using Shopizy.Application.Categories.Commands.CreateCategory;
using Shopizy.Application.Categories.Commands.DeleteCategory;
using Shopizy.Application.Categories.Commands.UpdateCategory;
using Shopizy.Application.Categories.Queries.CategoriesTree;
using Shopizy.Application.Categories.Queries.GetCategory;
using Shopizy.Contracts.Category;
using Shopizy.Domain.Categories;

namespace Shopizy.Api.Common.Mapping;

/// <summary>
/// Configures mapping for category-related models.
/// </summary>
public class CategoryMappingConfig : IRegister
{
    /// <summary>
    /// Registers the mapping configurations.
    /// </summary>
    /// <param name="config">The type adapter configuration.</param>
    public void Register(TypeAdapterConfig config)
    {
        Guard.Against.Null(config);

        config
            .NewConfig<(Guid UserId, CreateCategoryRequest Request), CreateCategoryCommand>()
            .Map(dest => dest, src => src.Request);

        config
            .NewConfig<(Guid UserId, Guid CategoryId, UpdateCategoryRequest Request), UpdateCategoryCommand>()
            .Map(dest => dest.UserId, src => src.UserId)
            .Map(dest => dest.CategoryId, src => src.CategoryId)
            .Map(dest => dest, src => src.Request);

        config
            .NewConfig<(Guid UserId, Guid CategoryId), DeleteCategoryCommand>()
            .Map(dest => dest.UserId, src => src.UserId)
            .Map(dest => dest.CategoryId, src => src.CategoryId);

        config.NewConfig<Category, CategoryResponse>().Map(dest => dest.Id, src => src.Id.Value);

        config
            .NewConfig<Category, CategoryTreeResponse>()
            .Map(dest => dest.Id, src => src.Id.Value);

        config.NewConfig<CategoryTree, CategoryTreeResponse>();

        config.NewConfig<Guid, GetCategoryQuery>().MapWith(src => new GetCategoryQuery(src));
    }
}
