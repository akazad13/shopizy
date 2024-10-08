using ErrorOr;
using MapsterMapper;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Shopizy.Application.Categories.Commands.CreateCategory;
using Shopizy.Application.Categories.Commands.DeleteCategory;
using Shopizy.Application.Categories.Commands.UpdateCategory;
using Shopizy.Application.Categories.Queries.GetCategory;
using Shopizy.Application.Categories.Queries.ListCategories;
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
        var query = new ListCategoriesQuery();
        ErrorOr<List<Domain.Categories.Category>> result = await _mediator.Send(query);

        return result.Match(category => Ok(_mapper.Map<List<CategoryResponse>>(category)), Problem);
    }

    [HttpGet("categories/{categoryId:guid}")]
    public async Task<IActionResult> GetCategoryAsync(Guid categoryId)
    {
        GetCategoryQuery query = _mapper.Map<GetCategoryQuery>(categoryId);
        ErrorOr<Domain.Categories.Category> result = await _mediator.Send(query);

        return result.Match(category => Ok(_mapper.Map<CategoryResponse>(category)), Problem);
    }

    [HttpPost("users/{userId:guid}/categories")]
    public async Task<IActionResult> CreateCategoryAsync(Guid userId, CreateCategoryRequest request)
    {
        CreateCategoryCommand command = _mapper.Map<CreateCategoryCommand>((userId, request));
        ErrorOr<Domain.Categories.Category> result = await _mediator.Send(command);

        return result.Match(category => Ok(_mapper.Map<CategoryResponse>(category)), Problem);
    }

    [HttpPatch("users/{userId:guid}/categories/{categoryId:guid}")]
    public async Task<IActionResult> UpdateCategoryAsync(
        Guid userId,
        Guid categoryId,
        UpdateCategoryRequest request
    )
    {
        UpdateCategoryCommand command = _mapper.Map<UpdateCategoryCommand>(
            (userId, categoryId, request)
        );
        ErrorOr<Domain.Categories.Category> result = await _mediator.Send(command);

        return result.Match(category => Ok(_mapper.Map<CategoryResponse>(category)), Problem);
    }

    [HttpDelete("users/{userId:guid}/categories/{categoryId:guid}")]
    public async Task<IActionResult> DeleteCategoryAsync(Guid userId, Guid categoryId)
    {
        DeleteCategoryCommand command = _mapper.Map<DeleteCategoryCommand>((userId, categoryId));
        ErrorOr<Success> result = await _mediator.Send(command);

        return result.Match(category => Ok(_mapper.Map<Success>(category)), Problem);
    }
}
