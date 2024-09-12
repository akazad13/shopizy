using MapsterMapper;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Shopizy.Application.Carts.Commands.AddProductToCart;
using Shopizy.Application.Carts.Commands.CreateCartWithFirstProduct;
using Shopizy.Application.Carts.Commands.RemoveProductFromCart;
using Shopizy.Application.Carts.Commands.UpdateProductQuantity;
using Shopizy.Application.Carts.Queries.GetCart;
using Shopizy.Contracts.Cart;

namespace Shopizy.Api.Controllers;

[Route("api/v1.0/users/{userId:guid}/carts")]
public class CartController(ISender mediator, IMapper mapper) : ApiController
{
    private readonly ISender _mediator = mediator;
    private readonly IMapper _mapper = mapper;

    [HttpGet]
    public async Task<IActionResult> GetCartAsync(Guid userId)
    {
        GetCartQuery query = _mapper.Map<GetCartQuery>(userId);
        ErrorOr.ErrorOr<Domain.Carts.Cart> result = await _mediator.Send(query);

        return result.Match(Product => Ok(_mapper.Map<CartResponse?>(Product)), Problem);
    }

    [HttpPost]
    public async Task<IActionResult> CreateCartWithFirstProductAsync(
        Guid userId,
        CreateCartWithFirstProductRequest request
    )
    {
        CreateCartWithFirstProductCommand command = _mapper.Map<CreateCartWithFirstProductCommand>(
            (userId, request)
        );
        ErrorOr.ErrorOr<Domain.Carts.Cart> result = await _mediator.Send(command);

        return result.Match(product => Ok(_mapper.Map<CartResponse>(product)), Problem);
    }

    [HttpPatch("{cartId:guid}/add-product")]
    public async Task<IActionResult> AddProductToCartAsync(
        Guid userId,
        Guid cartId,
        AddProductToCartRequest request
    )
    {
        AddProductToCartCommand command = _mapper.Map<AddProductToCartCommand>(
            (userId, cartId, request)
        );
        ErrorOr.ErrorOr<Domain.Carts.Cart> result = await _mediator.Send(command);

        return result.Match(product => Ok(_mapper.Map<CartResponse>(product)), Problem);
    }

    [HttpPatch("{cartId:guid}/update-quantity")]
    public async Task<IActionResult> UpdateProductQuantityAsync(
        Guid userId,
        Guid cartId,
        UpdateProductQuantityRequest request
    )
    {
        UpdateProductQuantityCommand command = _mapper.Map<UpdateProductQuantityCommand>(
            (userId, cartId, request)
        );
        ErrorOr.ErrorOr<ErrorOr.Success> result = await _mediator.Send(command);

        return result.Match(success => Ok(success), Problem);
    }

    [HttpDelete("{cartId:guid}/remove-products")]
    public async Task<IActionResult> RemoveProductFromCartAsync(
        Guid userId,
        Guid cartId,
        RemoveProductFromCartRequest request
    )
    {
        RemoveProductFromCartCommand command = _mapper.Map<RemoveProductFromCartCommand>(
            (userId, cartId, request)
        );
        ErrorOr.ErrorOr<ErrorOr.Success> result = await _mediator.Send(command);

        return result.Match(success => Ok(success), Problem);
    }
}
