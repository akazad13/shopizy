using MediatR;
using shopizy.Application.Categories.Queries.CategoriesTree;
using Shopizy.Application.Common.Interfaces.Persistence;
using Shopizy.Application.Common.Wrappers;

namespace Shopizy.Application.Categories.Queries.CategoriesTree;

public class CategoriesTreeQueryHandler(ICategoryRepository categoryRepository)
    : IRequestHandler<CategoriesTreeQuery, IResult<List<CategoryTree>>>
{
    private readonly ICategoryRepository _categoryRepository = categoryRepository;

    public async Task<IResult<List<CategoryTree>>> Handle(
        CategoriesTreeQuery query,
        CancellationToken cancellationToken
    )
    {
        var categories = await _categoryRepository.GetCategoriesAsync();

        var categoriesTree = BuildCategoryTree(
            [
                .. categories
                    .AsQueryable()
                    .Select(category => new CategoryTree()
                    {
                        Id = category.Id.Value,
                        Name = category.Name,
                        ParentId = category.ParentId,
                    }),
            ]
        );

        return Response<List<CategoryTree>>.SuccessResponese(categoriesTree);
    }

    private static List<CategoryTree> BuildCategoryTree(
        List<CategoryTree> allCategories,
        Guid? parentId = null
    )
    {
        // Get all categories with the given parentId
        var subCategories = allCategories.Where(c => c.ParentId == parentId).ToList();

        // Recursively build the tree by adding children to each category
        foreach (var category in subCategories)
        {
            category.Children = BuildCategoryTree(allCategories, category.Id);
        }

        return subCategories;
    }
}
