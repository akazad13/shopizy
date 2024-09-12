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

[Route("api/v1.0")]
public class ProductController(ISender mediator, IMapper mapper) : ApiController
{
    private readonly ISender _mediator = mediator;
    private readonly IMapper _mapper = mapper;

    [HttpGet("products")]
    public async Task<IActionResult> GetAsync()
    {
        var query = new ListProductQuery();
        ErrorOr<List<Domain.Products.Product>> result = await _mediator.Send(query);

        return result.Match(Product => Ok(_mapper.Map<List<ProductResponse>?>(Product)), Problem);
    }

    [HttpGet("products/{productId:guid}")]
    public async Task<IActionResult> GetProductAsync(Guid ProductId)
    {
        GetProductQuery query = _mapper.Map<GetProductQuery>(ProductId);
        ErrorOr<Domain.Products.Product> result = await _mediator.Send(query);

        return result.Match(Product => Ok(_mapper.Map<ProductResponse?>(Product)), Problem);
    }

    [HttpPost("users/{userId:guid}/products")]
    public async Task<IActionResult> CreateProductAsync(Guid userId, CreateProductRequest request)
    {
        CreateProductCommand command = _mapper.Map<CreateProductCommand>((userId, request));
        ErrorOr<Domain.Products.Product> result = await _mediator.Send(command);

        return result.Match(product => Ok(_mapper.Map<ProductResponse>(product)), Problem);
    }

    [HttpPatch("users/{userId:guid}/products/{productId:guid}")]
    public async Task<IActionResult> UpdateProductAsync(
        Guid userId,
        Guid productId,
        UpdateProductRequest request
    )
    {
        UpdateProductCommand command = _mapper.Map<UpdateProductCommand>(
            (userId, productId, request)
        );
        ErrorOr<Domain.Products.Product> result = await _mediator.Send(command);

        return result.Match(product => Ok(_mapper.Map<ProductResponse>(product)), Problem);
    }

    [HttpDelete("users/{userId:guid}/products/{productId:guid}")]
    public async Task<IActionResult> DeleteProductAsync(Guid userId, Guid productId)
    {
        DeleteProductCommand command = _mapper.Map<DeleteProductCommand>((userId, productId));
        ErrorOr<Success> result = await _mediator.Send(command);

        return result.Match(product => Ok(_mapper.Map<Success>(product)), Problem);
    }

    [HttpPost("users/{userId:guid}/products/{productId:guid}/image")]
    public async Task<IActionResult> AddProductImageAsync(
        Guid userId,
        Guid productId,
        [FromForm] AddProductImageRequest request
    )
    {
        var command = new AddProductImageCommand(userId, productId, request.File);
        ErrorOr<Domain.Products.Entities.ProductImage> result = await _mediator.Send(command);

        return result.Match(product => Ok(_mapper.Map<ProductImageResponse>(product)), Problem);
    }

    [HttpDelete("users/{userId:guid}/products/{productId:guid}/image/{imageId:guid}")]
    public async Task<IActionResult> DeleteProductImageAsync(
        Guid userId,
        Guid productId,
        Guid imageId
    )
    {
        DeleteProductImageCommand command = _mapper.Map<DeleteProductImageCommand>(
            (userId, productId, imageId)
        );
        ErrorOr<Success> result = await _mediator.Send(command);

        return result.Match(success => Ok(success), Problem);
    }
}
