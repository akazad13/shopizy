using MapsterMapper;
using MediatR;
using Microsoft.AspNetCore.Mvc;
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

[Route("api/v1.0")]
public class ProductController(ISender mediator, IMapper mapper) : ApiController
{
    private readonly ISender _mediator = mediator;
    private readonly IMapper _mapper = mapper;

    [HttpGet("products")]
    [SwaggerResponse(StatusCodes.Status200OK, null, typeof(List<ProductResponse>))]
    [SwaggerResponse(StatusCodes.Status400BadRequest, null, typeof(ErrorResult))]
    [SwaggerResponse(StatusCodes.Status401Unauthorized, null, typeof(ErrorResult))]
    [SwaggerResponse(StatusCodes.Status500InternalServerError, null, typeof(ErrorResult))]
    public async Task<IActionResult> SearchAsync([FromQuery] ProductsCriteriaRequest request)
    {
        var query = _mapper.Map<GetProductsQuery>(request);
        var result = await _mediator.Send(query);

        return result.Match(products => Ok(_mapper.Map<List<ProductResponse>>(products)), Problem);
    }

    [HttpGet("products/{productId:guid}")]
    [SwaggerResponse(StatusCodes.Status200OK, null, typeof(ProductDetailResponse))]
    [SwaggerResponse(StatusCodes.Status400BadRequest, null, typeof(ErrorResult))]
    [SwaggerResponse(StatusCodes.Status401Unauthorized, null, typeof(ErrorResult))]
    [SwaggerResponse(StatusCodes.Status500InternalServerError, null, typeof(ErrorResult))]
    public async Task<IActionResult> GetProductAsync(Guid ProductId)
    {
        var query = _mapper.Map<GetProductQuery>(ProductId);
        var result = await _mediator.Send(query);

        return result.Match(product => Ok(_mapper.Map<ProductDetailResponse>(product)), Problem);
    }

    [HttpPost("users/{userId:guid}/products")]
    [SwaggerResponse(StatusCodes.Status200OK, null, typeof(ProductResponse))]
    [SwaggerResponse(StatusCodes.Status400BadRequest, null, typeof(ErrorResult))]
    [SwaggerResponse(StatusCodes.Status401Unauthorized, null, typeof(ErrorResult))]
    [SwaggerResponse(StatusCodes.Status409Conflict, null, typeof(ErrorResult))]
    [SwaggerResponse(StatusCodes.Status500InternalServerError, null, typeof(ErrorResult))]
    public async Task<IActionResult> CreateProductAsync(Guid userId, CreateProductRequest request)
    {
        var command = _mapper.Map<CreateProductCommand>((userId, request));
        var result = await _mediator.Send(command);

        return result.Match(product => Ok(_mapper.Map<ProductResponse>(product)), Problem);
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
        var command = _mapper.Map<UpdateProductCommand>((userId, productId, request));
        var result = await _mediator.Send(command);

        return result.Match(
            success => Ok(SuccessResult.Success("Successfully updated product.")),
            Problem
        );
    }

    [HttpDelete("users/{userId:guid}/products/{productId:guid}")]
    [SwaggerResponse(StatusCodes.Status200OK, null, typeof(SuccessResult))]
    [SwaggerResponse(StatusCodes.Status400BadRequest, null, typeof(ErrorResult))]
    [SwaggerResponse(StatusCodes.Status401Unauthorized, null, typeof(ErrorResult))]
    [SwaggerResponse(StatusCodes.Status409Conflict, null, typeof(ErrorResult))]
    [SwaggerResponse(StatusCodes.Status500InternalServerError, null, typeof(ErrorResult))]
    public async Task<IActionResult> DeleteProductAsync(Guid userId, Guid productId)
    {
        var command = _mapper.Map<DeleteProductCommand>((userId, productId));
        var result = await _mediator.Send(command);

        return result.Match(
            success => Ok(SuccessResult.Success("Successfully deleted product.")),
            Problem
        );
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
        var command = new AddProductImageCommand(userId, productId, request.File);
        var result = await _mediator.Send(command);

        return result.Match(
            productImage => Ok(_mapper.Map<ProductImageResponse>(productImage)),
            Problem
        );
    }

    [HttpDelete("users/{userId:guid}/products/{productId:guid}/image/{imageId:guid}")]
    [SwaggerResponse(StatusCodes.Status200OK, null, typeof(SuccessResult))]
    [SwaggerResponse(StatusCodes.Status400BadRequest, null, typeof(ErrorResult))]
    [SwaggerResponse(StatusCodes.Status401Unauthorized, null, typeof(ErrorResult))]
    [SwaggerResponse(StatusCodes.Status409Conflict, null, typeof(ErrorResult))]
    [SwaggerResponse(StatusCodes.Status500InternalServerError, null, typeof(ErrorResult))]
    public async Task<IActionResult> DeleteProductImageAsync(
        Guid userId,
        Guid productId,
        Guid imageId
    )
    {
        var command = _mapper.Map<DeleteProductImageCommand>((userId, productId, imageId));
        var result = await _mediator.Send(command);

        return result.Match(
            success => Ok(SuccessResult.Success("Successfully deleted product image.")),
            Problem
        );
    }
}
