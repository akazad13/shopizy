using MapsterMapper;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Shopizy.Application.Carts.Commands.AddProductToCart;
using Shopizy.Application.Carts.Commands.CreateCartWithFirstProduct;
using Shopizy.Application.Carts.Commands.RemoveProductsFromCart;
using Shopizy.Application.Carts.Commands.UpdateProductQuantity;
using Shopizy.Application.Carts.Queries.GetCart;
using Shopizy.Contracts.Cart;

namespace Shopizy.Api.Controllers;

[Route("api/users/{userId:guid}/carts")]
public class CartController(ISender _mediator, IMapper _mapper) : ApiController
{
    [HttpGet]
    public async Task<IActionResult> GetCart(Guid userId)
    {
        var query = _mapper.Map<GetCartQuery>(userId);
        var result = await _mediator.Send(query);

        return result.Match(
            Product => Ok(_mapper.Map<CartResponse?>(Product)),
            Problem);
    }

    [HttpPost]
    public async Task<IActionResult> CreateCartWithFirstProduct(Guid userId, CreateCartWithFirstProductRequest request)
    {
        var command = _mapper.Map<CreateCartWithFirstProductCommand>((userId, request));
        var result = await _mediator.Send(command);

        return result.Match(
            product => Ok(_mapper.Map<CartResponse>(product)),
            Problem);
    }

    [HttpPatch("{cartId:guid}/add-product")]
    public async Task<IActionResult> AddProductToCart(Guid userId, Guid cartId, AddProductToCartRequest request)
    {
        var command = _mapper.Map<AddProductToCartCommand>((userId, cartId, request));
        var result = await _mediator.Send(command);

        return result.Match(
            product => Ok(_mapper.Map<CartResponse>(product)),
            Problem);
    }

    [HttpPatch("{cartId:guid}/update-quantity")]
    public async Task<IActionResult> UpdateProductQuantity(Guid userId, Guid cartId, UpdateProductQuantityRequest request)
    {
        var command = _mapper.Map<UpdateProductQuantityCommand>((userId, cartId, request));
        var result = await _mediator.Send(command);

        return result.Match(
            success => Ok(success),
            Problem);
    }

    [HttpDelete("{cartId:guid}/remove-products")]
    public async Task<IActionResult> RemoveProductFromCart(Guid userId, Guid cartId, RemoveProductFromCartRequest request)
    {
        var command = _mapper.Map<RemoveProductFromCartCommand>((userId, cartId, request));
        var result = await _mediator.Send(command);

        return result.Match(
            success => Ok(success),
            Problem);
    }
}