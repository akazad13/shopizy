using MapsterMapper;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Shopizy.Application.Products.Commands.CreateProduct;
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
    [HttpGet("products/{ProductId:guid}")]
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
}