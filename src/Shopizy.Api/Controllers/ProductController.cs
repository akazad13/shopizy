using ErrorOr;
using MapsterMapper;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using shopizy.Contracts.Product;
using Shopizy.Api.Common.LoggerMessages;
using Shopizy.Application.Products.Commands.AddProductImage;
using Shopizy.Application.Products.Commands.CreateProduct;
using Shopizy.Application.Products.Commands.DeleteProduct;
using Shopizy.Application.Products.Commands.DeleteProductImage;
using Shopizy.Application.Products.Commands.UpdateProduct;
using Shopizy.Application.Products.Queries.GetProduct;
using Shopizy.Application.Products.Queries.GetProducts;
using Shopizy.Contracts.Category;
using Shopizy.Contracts.Common;
using Shopizy.Contracts.Product;
using Swashbuckle.AspNetCore.Annotations;

namespace Shopizy.Api.Controllers;

/// <summary>
/// Controller for managing product operations.
/// </summary>
[Route("api/v1.0")]
public class ProductController(ISender mediator, IMapper mapper, ILogger<ProductController> logger)
    : ApiController
{
    private readonly ISender _mediator = mediator;
    private readonly IMapper _mapper = mapper;
    private readonly ILogger<ProductController> _logger = logger;

    /// <summary>
    /// Searches for products based on criteria.
    /// </summary>
    /// <param name="criteria">The search criteria including filters and pagination.</param>
    /// <returns>A list of products matching the criteria.</returns>
    /// <response code="200">Returns the list of products.</response>
    /// <response code="400">If the request is invalid.</response>
    /// <response code="401">If the user is not authenticated.</response>
    /// <response code="500">If an internal server error occurs.</response>
    [HttpGet("products")]
    [SwaggerResponse(StatusCodes.Status200OK, null, typeof(List<ProductResponse>))]
    [SwaggerResponse(StatusCodes.Status400BadRequest, null, typeof(ErrorResult))]
    [SwaggerResponse(StatusCodes.Status401Unauthorized, null, typeof(ErrorResult))]
    [SwaggerResponse(StatusCodes.Status500InternalServerError, null, typeof(ErrorResult))]
    public async Task<IActionResult> SearchAsync([FromQuery] ProductsCriteria criteria)
    {
        try
        {
            var query = _mapper.Map<GetProductsQuery>(criteria);
            var result = await _mediator.Send(query);

            return result.Match(
                products => Ok(_mapper.Map<List<ProductResponse>>(products)),
                Problem
            );
        }
        catch (Exception ex)
        {
            _logger.ProductFetchError(ex);
            return Problem([Error.Unexpected(description: ex.Message)]);
        }
    }

    [HttpGet("products/{productId:guid}")]
    [SwaggerResponse(StatusCodes.Status200OK, null, typeof(ProductDetailResponse))]
    [SwaggerResponse(StatusCodes.Status400BadRequest, null, typeof(ErrorResult))]
    [SwaggerResponse(StatusCodes.Status401Unauthorized, null, typeof(ErrorResult))]
    [SwaggerResponse(StatusCodes.Status500InternalServerError, null, typeof(ErrorResult))]
    public async Task<IActionResult> GetProductAsync(Guid productId)
    {
        try
        {
            var query = _mapper.Map<GetProductQuery>(productId);
            var result = await _mediator.Send(query);

            return result.Match(
                product => Ok(_mapper.Map<ProductDetailResponse>(product)),
                Problem
            );
        }
        catch (Exception ex)
        {
            _logger.ProductFetchError(ex);
            return Problem([Error.Unexpected(description: ex.Message)]);
        }
    }

    [HttpPost("users/{userId:guid}/products")]
    [SwaggerResponse(StatusCodes.Status200OK, null, typeof(ProductResponse))]
    [SwaggerResponse(StatusCodes.Status400BadRequest, null, typeof(ErrorResult))]
    [SwaggerResponse(StatusCodes.Status401Unauthorized, null, typeof(ErrorResult))]
    [SwaggerResponse(StatusCodes.Status409Conflict, null, typeof(ErrorResult))]
    [SwaggerResponse(StatusCodes.Status500InternalServerError, null, typeof(ErrorResult))]
    public async Task<IActionResult> CreateProductAsync(Guid userId, CreateProductRequest request)
    {
        try
        {
            var command = _mapper.Map<CreateProductCommand>((userId, request));
            var result = await _mediator.Send(command);

            return result.Match(product => Ok(_mapper.Map<ProductResponse>(product)), Problem);
        }
        catch (Exception ex)
        {
            _logger.ProductCreationError(ex);
            return Problem([Error.Unexpected(description: ex.Message)]);
        }
    }

    [HttpPatch("users/{userId:guid}/products/{productId:guid}")]
    [SwaggerResponse(StatusCodes.Status200OK, null, typeof(SuccessResult))]
    [SwaggerResponse(StatusCodes.Status400BadRequest, null, typeof(ErrorResult))]
    [SwaggerResponse(StatusCodes.Status401Unauthorized, null, typeof(ErrorResult))]
    [SwaggerResponse(StatusCodes.Status409Conflict, null, typeof(ErrorResult))]
    [SwaggerResponse(StatusCodes.Status500InternalServerError, null, typeof(ErrorResult))]
    public async Task<IActionResult> UpdateProductAsync(
        Guid userId,
        Guid productId,
        UpdateProductRequest request
    )
    {
        try
        {
            var command = _mapper.Map<UpdateProductCommand>((userId, productId, request));
            var result = await _mediator.Send(command);

            return result.Match(
                success => Ok(SuccessResult.Success("Successfully updated product.")),
                Problem
            );
        }
        catch (Exception ex)
        {
            _logger.ProductUpdateError(ex);
            return Problem([Error.Unexpected(description: ex.Message)]);
        }
    }

    [HttpDelete("users/{userId:guid}/products/{productId:guid}")]
    [SwaggerResponse(StatusCodes.Status200OK, null, typeof(SuccessResult))]
    [SwaggerResponse(StatusCodes.Status400BadRequest, null, typeof(ErrorResult))]
    [SwaggerResponse(StatusCodes.Status401Unauthorized, null, typeof(ErrorResult))]
    [SwaggerResponse(StatusCodes.Status409Conflict, null, typeof(ErrorResult))]
    [SwaggerResponse(StatusCodes.Status500InternalServerError, null, typeof(ErrorResult))]
    public async Task<IActionResult> DeleteProductAsync(Guid userId, Guid productId)
    {
        try
        {
            var command = _mapper.Map<DeleteProductCommand>((userId, productId));
            var result = await _mediator.Send(command);

            return result.Match(
                success => Ok(SuccessResult.Success("Successfully deleted product.")),
                Problem
            );
        }
        catch (Exception ex)
        {
            _logger.ProductDeleteError(ex);
            return Problem([Error.Unexpected(description: ex.Message)]);
        }
    }

    [HttpPost("users/{userId:guid}/products/{productId:guid}/image")]
    [SwaggerResponse(StatusCodes.Status200OK, null, typeof(ProductImageResponse))]
    [SwaggerResponse(StatusCodes.Status400BadRequest, null, typeof(ErrorResult))]
    [SwaggerResponse(StatusCodes.Status401Unauthorized, null, typeof(ErrorResult))]
    [SwaggerResponse(StatusCodes.Status409Conflict, null, typeof(ErrorResult))]
    [SwaggerResponse(StatusCodes.Status500InternalServerError, null, typeof(ErrorResult))]
    public async Task<IActionResult> AddProductImageAsync(
        Guid userId,
        Guid productId,
        [FromForm] AddProductImageRequest request
    )
    {
        try
        {
            var command = new AddProductImageCommand(userId, productId, request.File);
            var result = await _mediator.Send(command);

            return result.Match(
                productImage => Ok(_mapper.Map<ProductImageResponse>(productImage)),
                Problem
            );
        }
        catch (Exception ex)
        {
            _logger.ProductImageAdditionError(ex);
            return Problem([Error.Unexpected(description: ex.Message)]);
        }
    }

    [HttpDelete("users/{userId:guid}/products/{productId:guid}/image/{imageId:guid}")]
    [SwaggerResponse(StatusCodes.Status200OK, null, typeof(SuccessResult))]
    [SwaggerResponse(StatusCodes.Status400BadRequest, null, typeof(ErrorResult))]
    [SwaggerResponse(StatusCodes.Status401Unauthorized, null, typeof(ErrorResult))]
    [SwaggerResponse(StatusCodes.Status409Conflict, null, typeof(ErrorResult))]
    [SwaggerResponse(StatusCodes.Status500InternalServerError, null, typeof(ErrorResult))]
    public async Task<IActionResult> DeleteProductImageAsync(Guid userId, Guid productId, Guid imageId)
    {
        try
        {
            var command = _mapper.Map<DeleteProductImageCommand>((userId, productId, imageId));
            var result = await _mediator.Send(command);

            return result.Match(
                success => Ok(SuccessResult.Success("Successfully deleted product image.")),
                Problem
            );
        }
        catch (Exception ex)
        {
            _logger.ProductImageDeleteError(ex);
            return Problem([Error.Unexpected(description: ex.Message)]);
        }
    }
}
