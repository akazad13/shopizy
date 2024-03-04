using MapsterMapper;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using shopizy.Application.Categories.Commands.CreateCategory;
using shopizy.Contracts.Category;

namespace Shopizy.Api.Controllers;

[Route("api/category")]
public class CategoryController(ISender _mediator, IMapper _mapper) : ApiController
{
    [HttpPost]
    public async Task<IActionResult> Create(CreateCategoryRequest request)
    {
        var command = _mapper.Map<CreateCategoryCommand>(request);
        var categoryResult = await _mediator.Send(command);

        return categoryResult.Match(
            category => Ok(_mapper.Map<CategoryResponse>(category)),
            Problem);
    }   
}