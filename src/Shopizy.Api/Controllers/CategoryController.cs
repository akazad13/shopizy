using ErrorOr;
using MapsterMapper;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Shopizy.Api.Common.LoggerMessages;
using Shopizy.Application.Categories.Commands.CreateCategory;
using Shopizy.Application.Categories.Commands.DeleteCategory;
using Shopizy.Application.Categories.Commands.UpdateCategory;
using Shopizy.Application.Categories.Queries.CategoriesTree;
using Shopizy.Application.Categories.Queries.GetCategory;
using Shopizy.Application.Categories.Queries.ListCategories;
using shopizy.Contracts.Category;
using Shopizy.Contracts.Category;
using Shopizy.Contracts.Common;
using Swashbuckle.AspNetCore.Annotations;

namespace Shopizy.Api.Controllers;

[Route("api/v1.0")]
public class CategoryController(
    ISender mediator,
    IMapper mapper,
    ILogger<CategoryController> logger
) : ApiController
{
    private readonly ISender _mediator = mediator;
    private readonly IMapper _mapper = mapper;
    private readonly ILogger<CategoryController> _logger = logger;

    [HttpGet("categories")]
    [SwaggerResponse(StatusCodes.Status200OK, null, typeof(List<CategoryResponse>))]
    [SwaggerResponse(StatusCodes.Status400BadRequest, null, typeof(ErrorResult))]
    [SwaggerResponse(StatusCodes.Status401Unauthorized, null, typeof(ErrorResult))]
    [SwaggerResponse(StatusCodes.Status500InternalServerError, null, typeof(ErrorResult))]
    public async Task<IActionResult> GetAsync()
    {
        try
        {
            var result = await _mediator.Send(new ListCategoriesQuery());

            return result.Match(
                categories => Ok(_mapper.Map<List<CategoryResponse>>(categories)),
                Problem
            );
        }
        catch (Exception ex)
        {
            _logger.CategoryFetchError(ex);
            return Problem([Error.Unexpected(description: ex.Message)]);
        }
    }

    [HttpGet("categories/tree")]
    [SwaggerResponse(StatusCodes.Status200OK, null, typeof(List<CategoryTreeResponse>))]
    [SwaggerResponse(StatusCodes.Status400BadRequest, null, typeof(ErrorResult))]
    [SwaggerResponse(StatusCodes.Status401Unauthorized, null, typeof(ErrorResult))]
    [SwaggerResponse(StatusCodes.Status500InternalServerError, null, typeof(ErrorResult))]
    public async Task<IActionResult> GetCategoriesAsync()
    {
        try
        {
            var result = await _mediator.Send(new CategoriesTreeQuery());

            return result.Match(
                categories => Ok(_mapper.Map<List<CategoryTreeResponse>>(categories)),
                Problem
            );
        }
        catch (Exception ex)
        {
            _logger.CategoryFetchError(ex);
            return Problem([Error.Unexpected(description: ex.Message)]);
        }
    }

    [HttpGet("categories/{categoryId:guid}")]
    [SwaggerResponse(StatusCodes.Status200OK, null, typeof(CategoryResponse))]
    [SwaggerResponse(StatusCodes.Status400BadRequest, null, typeof(ErrorResult))]
    [SwaggerResponse(StatusCodes.Status401Unauthorized, null, typeof(ErrorResult))]
    [SwaggerResponse(StatusCodes.Status500InternalServerError, null, typeof(ErrorResult))]
    public async Task<IActionResult> GetCategoryAsync(Guid categoryId)
    {
        try
        {
            var query = _mapper.Map<GetCategoryQuery>(categoryId);
            var result = await _mediator.Send(query);

            return result.Match(category => Ok(_mapper.Map<CategoryResponse>(category)), Problem);
        }
        catch (Exception ex)
        {
            _logger.CategoryFetchError(ex);
            return Problem([Error.Unexpected(description: ex.Message)]);
        }
    }

    [HttpPost("users/{userId:guid}/categories")]
    [SwaggerResponse(StatusCodes.Status200OK, null, typeof(CategoryResponse))]
    [SwaggerResponse(StatusCodes.Status400BadRequest, null, typeof(ErrorResult))]
    [SwaggerResponse(StatusCodes.Status401Unauthorized, null, typeof(ErrorResult))]
    [SwaggerResponse(StatusCodes.Status409Conflict, null, typeof(ErrorResult))]
    [SwaggerResponse(StatusCodes.Status500InternalServerError, null, typeof(ErrorResult))]
    public async Task<IActionResult> CreateCategoryAsync(Guid userId, CreateCategoryRequest request)
    {
        try
        {
            var command = _mapper.Map<CreateCategoryCommand>((userId, request));
            var result = await _mediator.Send(command);

            return result.Match(category => Ok(_mapper.Map<CategoryResponse>(category)), Problem);
        }
        catch (Exception ex)
        {
            _logger.CategoryCreationError(ex);
            return Problem([Error.Unexpected(description: ex.Message)]);
        }
    }

    [HttpPatch("users/{userId:guid}/categories/{categoryId:guid}")]
    [SwaggerResponse(StatusCodes.Status200OK, null, typeof(SuccessResult))]
    [SwaggerResponse(StatusCodes.Status400BadRequest, null, typeof(ErrorResult))]
    [SwaggerResponse(StatusCodes.Status401Unauthorized, null, typeof(ErrorResult))]
    [SwaggerResponse(StatusCodes.Status409Conflict, null, typeof(ErrorResult))]
    [SwaggerResponse(StatusCodes.Status500InternalServerError, null, typeof(ErrorResult))]
    public async Task<IActionResult> UpdateCategoryAsync(
        Guid userId,
        Guid categoryId,
        UpdateCategoryRequest request
    )
    {
        try
        {
            var command = _mapper.Map<UpdateCategoryCommand>((userId, categoryId, request));
            var result = await _mediator.Send(command);

            return result.Match(
                success => Ok(SuccessResult.Success("Successfully updated category.")),
                Problem
            );
        }
        catch (Exception ex)
        {
            _logger.CategoryUpdateError(ex);
            return Problem([Error.Unexpected(description: ex.Message)]);
        }
    }

    [HttpDelete("users/{userId:guid}/categories/{categoryId:guid}")]
    [SwaggerResponse(StatusCodes.Status200OK, null, typeof(SuccessResult))]
    [SwaggerResponse(StatusCodes.Status400BadRequest, null, typeof(ErrorResult))]
    [SwaggerResponse(StatusCodes.Status401Unauthorized, null, typeof(ErrorResult))]
    [SwaggerResponse(StatusCodes.Status409Conflict, null, typeof(ErrorResult))]
    [SwaggerResponse(StatusCodes.Status500InternalServerError, null, typeof(ErrorResult))]
    public async Task<IActionResult> DeleteCategoryAsync(Guid userId, Guid categoryId)
    {
        try
        {
            var command = _mapper.Map<DeleteCategoryCommand>((userId, categoryId));
            var result = await _mediator.Send(command);

            return result.Match(
                success => Ok(SuccessResult.Success("Successfully deleted category.")),
                Problem
            );
        }
        catch (Exception ex)
        {
            _logger.CategoryDeleteError(ex);
            return Problem([Error.Unexpected(description: ex.Message)]);
        }
    }
}
