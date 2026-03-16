using Mapster;
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
        ArgumentNullException.ThrowIfNull(config);

        config.NewConfig<Category, CategoryResponse>().Map(dest => dest.Id, src => src.Id.Value);

        config
            .NewConfig<Category, CategoryTreeResponse>()
            .Map(dest => dest.Id, src => src.Id.Value);

        config.NewConfig<CategoryTree, CategoryTreeResponse>();

        config.NewConfig<Guid, GetCategoryQuery>().MapWith(src => new GetCategoryQuery(src));
    }
}
