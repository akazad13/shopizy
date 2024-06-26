using ErrorOr;
using MapsterMapper;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Shopizy.Application.Products.Commands.AddProductImage;
using Shopizy.Application.Products.Commands.CreateProduct;
using Shopizy.Application.Products.Commands.DeleteProduct;
using Shopizy.Application.Products.Commands.DeleteProductImage;
using Shopizy.Application.Products.Commands.UpdateProduct;
using Shopizy.Application.Products.Queries.GetProduct;
using Shopizy.Application.Products.Queries.ListProducts;
using Shopizy.Contracts.Product;

namespace Shopizy.Api.Controllers;

public class ProductController(ISender _mediator, IMapper _mapper) : ApiController
{
    [HttpGet("products")]
    public async Task<IActionResult> Get()
    {
        var query = new ListProductQuery();
        var result = await _mediator.Send(query);

        return result.Match(
            Product => Ok(_mapper.Map<List<ProductResponse>?>(Product)),
            Problem);
    }
    [HttpGet("products/{productId:guid}")]
    public async Task<IActionResult> GetProduct(Guid ProductId)
    {
        var query = _mapper.Map<GetProductQuery>(ProductId);
        var result = await _mediator.Send(query);

        return result.Match(
            Product => Ok(_mapper.Map<ProductResponse?>(Product)),
            Problem);
    }

    [HttpPost("users/{userId:guid}/products")]
    public async Task<IActionResult> CreateProduct(Guid userId, CreateProductRequest request)
    {
        var command = _mapper.Map<CreateProductCommand>((userId, request));
        var result = await _mediator.Send(command);

        return result.Match(
            product => Ok(_mapper.Map<ProductResponse>(product)),
            Problem);
    }

    [HttpPatch("users/{userId:guid}/products/{productId:guid}")]
    public async Task<IActionResult> UpdateProduct(Guid userId, Guid productId, UpdateProductRequest request)
    {
        var command = _mapper.Map<UpdateProductCommand>((userId, productId, request));
        var result = await _mediator.Send(command);

        return result.Match(
            product => Ok(_mapper.Map<ProductResponse>(product)),
            Problem);
    }

    [HttpDelete("users/{userId:guid}/products/{productId:guid}")]
    public async Task<IActionResult> DeleteProduct(Guid userId, Guid productId)
    {
        var command = _mapper.Map<DeleteProductCommand>((userId, productId));
        var result = await _mediator.Send(command);

        return result.Match(
            product => Ok(_mapper.Map<Success>(product)),
            Problem);
    }

    [HttpPost("users/{userId:guid}/products/{productId:guid}/image")]
    public async Task<IActionResult> AddProductImage(Guid userId, Guid productId, [FromForm] AddProductImageRequest request)
    {
        var command = new AddProductImageCommand(userId, productId, request.File);
        var result = await _mediator.Send(command);

        return result.Match(
            product => Ok(_mapper.Map<ProductImageResponse>(product)),
            Problem);
    }
    [HttpDelete("users/{userId:guid}/products/{productId:guid}/image/{imageId:guid}")]
    public async Task<IActionResult> DeleteProductImage(Guid userId, Guid productId, Guid imageId)
    {
        var command = _mapper.Map<DeleteProductImageCommand>((userId, productId, imageId));
        var result = await _mediator.Send(command);

        return result.Match(
            success => Ok(success),
            Problem);
    }
}