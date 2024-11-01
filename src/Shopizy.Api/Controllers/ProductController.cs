using MapsterMapper;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Shopizy.Application.Common.Wrappers;
using Shopizy.Application.Products.Commands.AddProductImage;
using Shopizy.Application.Products.Commands.CreateProduct;
using Shopizy.Application.Products.Commands.DeleteProduct;
using Shopizy.Application.Products.Commands.DeleteProductImage;
using Shopizy.Application.Products.Commands.UpdateProduct;
using Shopizy.Application.Products.Queries.GetProduct;
using Shopizy.Application.Products.Queries.ListProducts;
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
    [SwaggerResponse(StatusCodes.Status400BadRequest, null, typeof(GenericResponse))]
    [SwaggerResponse(StatusCodes.Status401Unauthorized, null, typeof(GenericResponse))]
    [SwaggerResponse(StatusCodes.Status500InternalServerError, null, typeof(GenericResponse))]
    public async Task<IActionResult> GetAsync([FromQuery] ProductsCriteriaRequest request)
    {
        var query = _mapper.Map<ListProductsQuery>(request);
        var result = await _mediator.Send(query);

        return result.Match<IActionResult>(
            Product => Ok(_mapper.Map<List<ProductResponse>?>(Product)),
            BadRequest
        );
    }

    [HttpGet("products/{productId:guid}")]
    [SwaggerResponse(StatusCodes.Status200OK, null, typeof(ProductResponse))]
    [SwaggerResponse(StatusCodes.Status400BadRequest, null, typeof(GenericResponse))]
    [SwaggerResponse(StatusCodes.Status401Unauthorized, null, typeof(GenericResponse))]
    [SwaggerResponse(StatusCodes.Status500InternalServerError, null, typeof(GenericResponse))]
    public async Task<IActionResult> GetProductAsync(Guid ProductId)
    {
        var query = _mapper.Map<GetProductQuery>(ProductId);
        var result = await _mediator.Send(query);

        return result.Match<IActionResult>(
            Product => Ok(_mapper.Map<ProductResponse?>(Product)),
            BadRequest
        );
    }

    [HttpPost("users/{userId:guid}/products")]
    [SwaggerResponse(StatusCodes.Status200OK, null, typeof(ProductResponse))]
    [SwaggerResponse(StatusCodes.Status400BadRequest, null, typeof(GenericResponse))]
    [SwaggerResponse(StatusCodes.Status401Unauthorized, null, typeof(GenericResponse))]
    [SwaggerResponse(StatusCodes.Status409Conflict, null, typeof(GenericResponse))]
    [SwaggerResponse(StatusCodes.Status500InternalServerError, null, typeof(GenericResponse))]
    public async Task<IActionResult> CreateProductAsync(Guid userId, CreateProductRequest request)
    {
        var command = _mapper.Map<CreateProductCommand>((userId, request));
        var result = await _mediator.Send(command);

        return result.Match<IActionResult>(
            product => Ok(_mapper.Map<ProductResponse>(product)),
            BadRequest
        );
    }

    [HttpPatch("users/{userId:guid}/products/{productId:guid}")]
    [SwaggerResponse(StatusCodes.Status200OK, null, typeof(ProductResponse))]
    [SwaggerResponse(StatusCodes.Status400BadRequest, null, typeof(GenericResponse))]
    [SwaggerResponse(StatusCodes.Status401Unauthorized, null, typeof(GenericResponse))]
    [SwaggerResponse(StatusCodes.Status409Conflict, null, typeof(GenericResponse))]
    [SwaggerResponse(StatusCodes.Status500InternalServerError, null, typeof(GenericResponse))]
    public async Task<IActionResult> UpdateProductAsync(
        Guid userId,
        Guid productId,
        UpdateProductRequest request
    )
    {
        var command = _mapper.Map<UpdateProductCommand>((userId, productId, request));
        var result = await _mediator.Send(command);

        return result.Match<IActionResult>(
            product => Ok(_mapper.Map<ProductResponse>(product)),
            BadRequest
        );
    }

    [HttpDelete("users/{userId:guid}/products/{productId:guid}")]
    [SwaggerResponse(StatusCodes.Status200OK, null, typeof(GenericResponse))]
    [SwaggerResponse(StatusCodes.Status400BadRequest, null, typeof(GenericResponse))]
    [SwaggerResponse(StatusCodes.Status401Unauthorized, null, typeof(GenericResponse))]
    [SwaggerResponse(StatusCodes.Status409Conflict, null, typeof(GenericResponse))]
    [SwaggerResponse(StatusCodes.Status500InternalServerError, null, typeof(GenericResponse))]
    public async Task<IActionResult> DeleteProductAsync(Guid userId, Guid productId)
    {
        var command = _mapper.Map<DeleteProductCommand>((userId, productId));
        var result = await _mediator.Send(command);

        return result.Match<IActionResult>(
            product => Ok(_mapper.Map<GenericResponse>(product)),
            BadRequest
        );
    }

    [HttpPost("users/{userId:guid}/products/{productId:guid}/image")]
    [SwaggerResponse(StatusCodes.Status200OK, null, typeof(ProductImageResponse))]
    [SwaggerResponse(StatusCodes.Status400BadRequest, null, typeof(GenericResponse))]
    [SwaggerResponse(StatusCodes.Status401Unauthorized, null, typeof(GenericResponse))]
    [SwaggerResponse(StatusCodes.Status409Conflict, null, typeof(GenericResponse))]
    [SwaggerResponse(StatusCodes.Status500InternalServerError, null, typeof(GenericResponse))]
    public async Task<IActionResult> AddProductImageAsync(
        Guid userId,
        Guid productId,
        [FromForm] AddProductImageRequest request
    )
    {
        var command = new AddProductImageCommand(userId, productId, request.File);
        var result = await _mediator.Send(command);

        return result.Match<IActionResult>(
            product => Ok(_mapper.Map<ProductImageResponse>(product)),
            BadRequest
        );
    }

    [HttpDelete("users/{userId:guid}/products/{productId:guid}/image/{imageId:guid}")]
    [SwaggerResponse(StatusCodes.Status200OK, null, typeof(GenericResponse))]
    [SwaggerResponse(StatusCodes.Status400BadRequest, null, typeof(GenericResponse))]
    [SwaggerResponse(StatusCodes.Status401Unauthorized, null, typeof(GenericResponse))]
    [SwaggerResponse(StatusCodes.Status409Conflict, null, typeof(GenericResponse))]
    [SwaggerResponse(StatusCodes.Status500InternalServerError, null, typeof(GenericResponse))]
    public async Task<IActionResult> DeleteProductImageAsync(
        Guid userId,
        Guid productId,
        Guid imageId
    )
    {
        var command = _mapper.Map<DeleteProductImageCommand>((userId, productId, imageId));
        var result = await _mediator.Send(command);

        return result.Match<IActionResult>(success => Ok(success), BadRequest);
    }
}
