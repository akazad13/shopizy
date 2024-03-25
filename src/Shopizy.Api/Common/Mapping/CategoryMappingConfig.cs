using Mapster;
using Shopizy.Application.Categories.Commands.CreateCategory;
using Shopizy.Application.Categories.Queries.GetCategory;
using Shopizy.Contracts.Category;
using Shopizy.Domain.Categories;

namespace Shopizy.Api.Common.Mapping;

public class CategoryMappingConfig : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config
            .NewConfig<(Guid UserId, CreateCategoryRequest Request), CreateCategoryCommand>()
            .Map(dest => dest.UserId, src => src.UserId)
            .Map(dest => dest, src => src.Request);

        config.NewConfig<Category, CategoryResponse>().Map(dest => dest.Id, src => src.Id.Value);

        config.NewConfig<Guid, GetCategoryQuery>().MapWith(src => new GetCategoryQuery(src));
    }
}


// Map(dest => dest.FullName, src => $"{src.Title} {src.FirstName} {src.LastName}")
//       .Map(dest => dest.Age,
//             src => DateTime.Now.Year - src.DateOfBirth.Value.Year,
//             srcCond => srcCond.DateOfBirth.HasValue);
