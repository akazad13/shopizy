using ErrorOr;
using MediatR;
using Shopizy.Application.Categories.Queries.CategoriesTree;
using Shopizy.Application.Common.Interfaces.Persistence;

namespace Shopizy.Application.Categories.Queries.CategoriesTree;

public class CategoriesTreeQueryHandler(ICategoryRepository categoryRepository)
    : IRequestHandler<CategoriesTreeQuery, ErrorOr<List<CategoryTree>>>
{
    private readonly ICategoryRepository _categoryRepository = categoryRepository;

    public async Task<ErrorOr<List<CategoryTree>>> Handle(
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

        return categoriesTree;
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
