using ErrorOr;
using MapsterMapper;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Shopizy.Api.Common.LoggerMessages;
using Shopizy.Application.Products.Commands.AddProductImage;
using Shopizy.Application.Products.Commands.CreateProduct;
using Shopizy.Application.Products.Commands.DeleteProduct;
using Shopizy.Application.Products.Commands.DeleteProductImage;
using Shopizy.Application.Products.Commands.UpdateProduct;
using Shopizy.Application.Products.Queries.GetProduct;
using Shopizy.Application.Products.Queries.GetProducts;
using Shopizy.Contracts.Common;
using shopizy.Contracts.Product;
using Shopizy.Contracts.Product;
using Swashbuckle.AspNetCore.Annotations;

namespace Shopizy.Api.Controllers;

[Route("api/v1.0/products")]
public class ProductController(ISender mediator, IMapper mapper, ILogger<ProductController> logger)
    : ApiController
{
    private readonly ISender _mediator = mediator;
    private readonly IMapper _mapper = mapper;
    private readonly ILogger<ProductController> _logger = logger;

    [HttpGet]
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

    [HttpGet("{productId:guid}")]
    [SwaggerResponse(StatusCodes.Status200OK, null, typeof(ProductDetailResponse))]
    [SwaggerResponse(StatusCodes.Status400BadRequest, null, typeof(ErrorResult))]
    [SwaggerResponse(StatusCodes.Status401Unauthorized, null, typeof(ErrorResult))]
    [SwaggerResponse(StatusCodes.Status500InternalServerError, null, typeof(ErrorResult))]
    public async Task<IActionResult> GetProductAsync(Guid ProductId)
    {
        try
        {
            var query = _mapper.Map<GetProductQuery>(ProductId);
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

    [HttpPost]
    [SwaggerResponse(StatusCodes.Status200OK, null, typeof(ProductResponse))]
    [SwaggerResponse(StatusCodes.Status400BadRequest, null, typeof(ErrorResult))]
    [SwaggerResponse(StatusCodes.Status401Unauthorized, null, typeof(ErrorResult))]
    [SwaggerResponse(StatusCodes.Status409Conflict, null, typeof(ErrorResult))]
    [SwaggerResponse(StatusCodes.Status500InternalServerError, null, typeof(ErrorResult))]
    public async Task<IActionResult> CreateProductAsync(CreateProductRequest request)
    {
        try
        {
            var command = _mapper.Map<CreateProductCommand>(request);
            var result = await _mediator.Send(command);

            return result.Match(product => Ok(_mapper.Map<ProductResponse>(product)), Problem);
        }
        catch (Exception ex)
        {
            _logger.ProductCreationError(ex);
            return Problem([Error.Unexpected(description: ex.Message)]);
        }
    }

    [HttpPatch("{productId:guid}")]
    [SwaggerResponse(StatusCodes.Status200OK, null, typeof(SuccessResult))]
    [SwaggerResponse(StatusCodes.Status400BadRequest, null, typeof(ErrorResult))]
    [SwaggerResponse(StatusCodes.Status401Unauthorized, null, typeof(ErrorResult))]
    [SwaggerResponse(StatusCodes.Status409Conflict, null, typeof(ErrorResult))]
    [SwaggerResponse(StatusCodes.Status500InternalServerError, null, typeof(ErrorResult))]
    public async Task<IActionResult> UpdateProductAsync(
        Guid productId,
        UpdateProductRequest request
    )
    {
        try
        {
            var command = _mapper.Map<UpdateProductCommand>((productId, request));
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

    [HttpDelete("{productId:guid}")]
    [SwaggerResponse(StatusCodes.Status200OK, null, typeof(SuccessResult))]
    [SwaggerResponse(StatusCodes.Status400BadRequest, null, typeof(ErrorResult))]
    [SwaggerResponse(StatusCodes.Status401Unauthorized, null, typeof(ErrorResult))]
    [SwaggerResponse(StatusCodes.Status409Conflict, null, typeof(ErrorResult))]
    [SwaggerResponse(StatusCodes.Status500InternalServerError, null, typeof(ErrorResult))]
    public async Task<IActionResult> DeleteProductAsync(Guid productId)
    {
        try
        {
            var command = _mapper.Map<DeleteProductCommand>(productId);
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

    [HttpPost("{productId:guid}/image")]
    [SwaggerResponse(StatusCodes.Status200OK, null, typeof(ProductImageResponse))]
    [SwaggerResponse(StatusCodes.Status400BadRequest, null, typeof(ErrorResult))]
    [SwaggerResponse(StatusCodes.Status401Unauthorized, null, typeof(ErrorResult))]
    [SwaggerResponse(StatusCodes.Status409Conflict, null, typeof(ErrorResult))]
    [SwaggerResponse(StatusCodes.Status500InternalServerError, null, typeof(ErrorResult))]
    public async Task<IActionResult> AddProductImageAsync(
        Guid productId,
        [FromForm] AddProductImageRequest request
    )
    {
        try
        {
            var command = new AddProductImageCommand(productId, request.File);
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

    [HttpDelete("{productId:guid}/image/{imageId:guid}")]
    [SwaggerResponse(StatusCodes.Status200OK, null, typeof(SuccessResult))]
    [SwaggerResponse(StatusCodes.Status400BadRequest, null, typeof(ErrorResult))]
    [SwaggerResponse(StatusCodes.Status401Unauthorized, null, typeof(ErrorResult))]
    [SwaggerResponse(StatusCodes.Status409Conflict, null, typeof(ErrorResult))]
    [SwaggerResponse(StatusCodes.Status500InternalServerError, null, typeof(ErrorResult))]
    public async Task<IActionResult> DeleteProductImageAsync(Guid productId, Guid imageId)
    {
        try
        {
            var command = _mapper.Map<DeleteProductImageCommand>((productId, imageId));
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
