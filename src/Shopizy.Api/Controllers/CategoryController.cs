using ErrorOr;
using MapsterMapper;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Shopizy.Application.Categories.Commands.CreateCategory;
using Shopizy.Application.Categories.Commands.DeleteCategory;
using Shopizy.Application.Categories.Commands.UpdateCategory;
using Shopizy.Application.Categories.Queries.GetCategory;
using Shopizy.Application.Categories.Queries.ListCategories;
using shopizy.Contracts.Category;
using Shopizy.Contracts.Category;

namespace Shopizy.Api.Controllers;

[Route("api/v1.0")]
public class CategoryController(ISender mediator, IMapper mapper) : ApiController
{
    private readonly ISender _mediator = mediator;
    private readonly IMapper _mapper = mapper;

    [HttpGet("categories")]
    public async Task<IActionResult> GetAsync()
    {
        var result = await _mediator.Send(new ListCategoriesQuery());

        return result.Match(category => Ok(_mapper.Map<List<CategoryResponse>>(category)), Problem);
    }

    [HttpGet("categories-tree")]
    public async Task<IActionResult> GetCategoriesAsync()
    {
        var result = await _mediator.Send(new ListCategoriesQuery());

        var categoriesModel = _mapper.Map<List<CategoryTreeResponse>>(result.Value);

        var categoryTree = BuildCategoryTree(categoriesModel);

        return Ok(categoryTree);
    }

    [HttpGet("categories/{categoryId:guid}")]
    public async Task<IActionResult> GetCategoryAsync(Guid categoryId)
    {
        var query = _mapper.Map<GetCategoryQuery>(categoryId);
        var result = await _mediator.Send(query);

        return result.Match(category => Ok(_mapper.Map<CategoryResponse>(category)), Problem);
    }

    [HttpPost("users/{userId:guid}/categories")]
    public async Task<IActionResult> CreateCategoryAsync(Guid userId, CreateCategoryRequest request)
    {
        var command = _mapper.Map<CreateCategoryCommand>((userId, request));
        var result = await _mediator.Send(command);

        return result.Match(category => Ok(_mapper.Map<CategoryResponse>(category)), Problem);
    }

    [HttpPatch("users/{userId:guid}/categories/{categoryId:guid}")]
    public async Task<IActionResult> UpdateCategoryAsync(
        Guid userId,
        Guid categoryId,
        UpdateCategoryRequest request
    )
    {
        var command = _mapper.Map<UpdateCategoryCommand>((userId, categoryId, request));
        var result = await _mediator.Send(command);

        return result.Match(category => Ok(_mapper.Map<CategoryResponse>(category)), Problem);
    }

    [HttpDelete("users/{userId:guid}/categories/{categoryId:guid}")]
    public async Task<IActionResult> DeleteCategoryAsync(Guid userId, Guid categoryId)
    {
        var command = _mapper.Map<DeleteCategoryCommand>((userId, categoryId));
        var result = await _mediator.Send(command);

        return result.Match(category => Ok(_mapper.Map<Success>(category)), Problem);
    }

    private static IEnumerable<CategoryTreeResponse> BuildCategoryTree(
        List<CategoryTreeResponse> allCategories,
        Guid? parentId = null
    )
    {
        // Get all categories with the given parentId
        var subCategories = allCategories.Where(c => c.ParentId == parentId);

        // Recursively build the tree by adding children to each category
        foreach (var category in subCategories)
        {
            category.Children = BuildCategoryTree(allCategories, category.Id);
        }

        return subCategories;
    }
}
