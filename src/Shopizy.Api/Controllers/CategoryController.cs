using MapsterMapper;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Shopizy.Application.Categories.Commands.CreateCategory;
using Shopizy.Application.Categories.Queries.GetCategory;
using Shopizy.Application.Categories.Queries.ListCategories;
using Shopizy.Contracts.Category;

namespace Shopizy.Api.Controllers;

public class CategoryController(ISender _mediator, IMapper _mapper) : ApiController
{
    [HttpGet("categories")]
    public async Task<IActionResult> Get()
    {
        var query = new ListCategoriesQuery();
        var result = await _mediator.Send(query);

        return result.Match(
            category => Ok(_mapper.Map<List<CategoryResponse>>(category)),
            Problem);
    }
    [HttpGet("categories/{categoryId:guid}")]
    public async Task<IActionResult> GetCategory(Guid categoryId)
    {
        var query = _mapper.Map<GetCategoryQuery>(categoryId);
        var result = await _mediator.Send(query);

        return result.Match(
            category => Ok(_mapper.Map<CategoryResponse>(category)),
            Problem);
    }

    [HttpPost("users/{userId:guid}/categories")]
    public async Task<IActionResult> CreateCategory(Guid userId, CreateCategoryRequest request)
    {
        var command = _mapper.Map<CreateCategoryCommand>((userId, request));
        var result = await _mediator.Send(command);

        return result.Match(
            category => Ok(_mapper.Map<CategoryResponse>(category)),
            Problem);
    }
}