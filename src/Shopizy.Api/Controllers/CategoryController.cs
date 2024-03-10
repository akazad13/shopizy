using MapsterMapper;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using shopizy.Application.Categories.Commands.CreateCategory;
using shopizy.Application.Categories.Queries.GetCategory;
using shopizy.Application.Categories.Queries.ListCategoriesQuery;
using shopizy.Contracts.Category;

namespace Shopizy.Api.Controllers;

[Route("api/users/{userId:guid}/categories")]
public class CategoryController(ISender _mediator, IMapper _mapper) : ApiController
{
    [HttpGet]
    public async Task<IActionResult> Get(Guid userId)
    {
        var command = new ListCategoriesQuery(userId);

        var categoryResult = await _mediator.Send(command);

        return categoryResult.Match(
            category => Ok(_mapper.Map<List<CategoryResponse>>(category)),
            Problem);
    }
    [HttpGet("{categoryId:guid}")]
    public async Task<IActionResult> GetCategory(Guid userId, Guid categoryId)
    {
         var command = _mapper.Map<GetCategoryQuery>((userId, categoryId));
        var categoryResult = await _mediator.Send(command);

        return categoryResult.Match(
            category => Ok(_mapper.Map<CategoryResponse>(category)),
            Problem);
    }
    
    [HttpPost]
    public async Task<IActionResult> CreateCategory(Guid userId, CreateCategoryRequest request)
    {
        var command = _mapper.Map<CreateCategoryCommand>((userId, request));
        var categoryResult = await _mediator.Send(command);

        return categoryResult.Match(
            category => Ok(_mapper.Map<CategoryResponse>(category)),
            Problem);
    }   
}