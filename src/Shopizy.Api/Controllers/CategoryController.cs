using MapsterMapper;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Shopizy.Application.Categories.Commands.CreateCategory;
using Shopizy.Application.Categories.Commands.DeleteCategory;
using Shopizy.Application.Categories.Commands.UpdateCategory;
using Shopizy.Application.Categories.Queries.CategoriesTree;
using Shopizy.Application.Categories.Queries.GetCategory;
using Shopizy.Application.Categories.Queries.ListCategories;
using Shopizy.Application.Common.Wrappers;
using shopizy.Contracts.Category;
using Shopizy.Contracts.Category;
using Swashbuckle.AspNetCore.Annotations;

namespace Shopizy.Api.Controllers;

[Route("api/v1.0")]
public class CategoryController(ISender mediator, IMapper mapper) : ApiController
{
    private readonly ISender _mediator = mediator;
    private readonly IMapper _mapper = mapper;

    [HttpGet("categories")]
    [SwaggerResponse(StatusCodes.Status200OK, null, typeof(List<CategoryResponse>))]
    [SwaggerResponse(StatusCodes.Status400BadRequest, null, typeof(GenericResponse))]
    [SwaggerResponse(StatusCodes.Status401Unauthorized, null, typeof(GenericResponse))]
    [SwaggerResponse(StatusCodes.Status500InternalServerError, null, typeof(GenericResponse))]
    public async Task<IActionResult> GetAsync()
    {
        var result = await _mediator.Send(new ListCategoriesQuery());

        return result.Match<IActionResult>(
            categories => Ok(_mapper.Map<List<CategoryResponse>>(categories)),
            BadRequest
        );
    }

    [HttpGet("categories/tree")]
    [SwaggerResponse(StatusCodes.Status200OK, null, typeof(List<CategoryTreeResponse>))]
    [SwaggerResponse(StatusCodes.Status400BadRequest, null, typeof(GenericResponse))]
    [SwaggerResponse(StatusCodes.Status401Unauthorized, null, typeof(GenericResponse))]
    [SwaggerResponse(StatusCodes.Status500InternalServerError, null, typeof(GenericResponse))]
    public async Task<IActionResult> GetCategoriesAsync()
    {
        var result = await _mediator.Send(new CategoriesTreeQuery());

        return result.Match<IActionResult>(
            categories => Ok(_mapper.Map<List<CategoryTreeResponse>>(categories)),
            BadRequest
        );
    }

    [HttpGet("categories/{categoryId:guid}")]
    [SwaggerResponse(StatusCodes.Status200OK, null, typeof(CategoryResponse))]
    [SwaggerResponse(StatusCodes.Status400BadRequest, null, typeof(GenericResponse))]
    [SwaggerResponse(StatusCodes.Status401Unauthorized, null, typeof(GenericResponse))]
    [SwaggerResponse(StatusCodes.Status500InternalServerError, null, typeof(GenericResponse))]
    public async Task<IActionResult> GetCategoryAsync(Guid categoryId)
    {
        var query = _mapper.Map<GetCategoryQuery>(categoryId);
        var result = await _mediator.Send(query);

        return result.Match<IActionResult>(
            category => Ok(_mapper.Map<CategoryResponse>(category)),
            BadRequest
        );
    }

    [HttpPost("users/{userId:guid}/categories")]
    [SwaggerResponse(StatusCodes.Status200OK, null, typeof(CategoryResponse))]
    [SwaggerResponse(StatusCodes.Status400BadRequest, null, typeof(GenericResponse))]
    [SwaggerResponse(StatusCodes.Status401Unauthorized, null, typeof(GenericResponse))]
    [SwaggerResponse(StatusCodes.Status409Conflict, null, typeof(GenericResponse))]
    [SwaggerResponse(StatusCodes.Status500InternalServerError, null, typeof(GenericResponse))]
    public async Task<IActionResult> CreateCategoryAsync(Guid userId, CreateCategoryRequest request)
    {
        var command = _mapper.Map<CreateCategoryCommand>((userId, request));
        var result = await _mediator.Send(command);

        return result.Match<IActionResult>(
            category => Ok(_mapper.Map<CategoryResponse>(category)),
            BadRequest
        );
    }

    [HttpPatch("users/{userId:guid}/categories/{categoryId:guid}")]
    [SwaggerResponse(StatusCodes.Status200OK, null, typeof(CategoryResponse))]
    [SwaggerResponse(StatusCodes.Status400BadRequest, null, typeof(GenericResponse))]
    [SwaggerResponse(StatusCodes.Status401Unauthorized, null, typeof(GenericResponse))]
    [SwaggerResponse(StatusCodes.Status409Conflict, null, typeof(GenericResponse))]
    [SwaggerResponse(StatusCodes.Status500InternalServerError, null, typeof(GenericResponse))]
    public async Task<IActionResult> UpdateCategoryAsync(
        Guid userId,
        Guid categoryId,
        UpdateCategoryRequest request
    )
    {
        var command = _mapper.Map<UpdateCategoryCommand>((userId, categoryId, request));
        var result = await _mediator.Send(command);

        return result.Match<IActionResult>(
            category => Ok(_mapper.Map<CategoryResponse>(category)),
            BadRequest
        );
    }

    [HttpDelete("users/{userId:guid}/categories/{categoryId:guid}")]
    [SwaggerResponse(StatusCodes.Status200OK, null, typeof(GenericResponse))]
    [SwaggerResponse(StatusCodes.Status400BadRequest, null, typeof(GenericResponse))]
    [SwaggerResponse(StatusCodes.Status401Unauthorized, null, typeof(GenericResponse))]
    [SwaggerResponse(StatusCodes.Status409Conflict, null, typeof(GenericResponse))]
    [SwaggerResponse(StatusCodes.Status500InternalServerError, null, typeof(GenericResponse))]
    public async Task<IActionResult> DeleteCategoryAsync(Guid userId, Guid categoryId)
    {
        try
        {
            var command = _mapper.Map<DeleteCategoryCommand>((userId, categoryId));
            var result = await _mediator.Send(command);

            return result.Match<IActionResult>(
                success => Ok(_mapper.Map<GenericResponse>(success)),
                BadRequest
            );
        }
        catch (Exception ex)
        {
            return StatusCode(
                StatusCodes.Status500InternalServerError,
                new GenericResponse(ex.Message, [ex.InnerException.Message])
            );
        }
    }
}
