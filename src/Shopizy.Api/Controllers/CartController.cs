using MapsterMapper;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Shopizy.Application.Carts.Commands.AddProductToCart;
using Shopizy.Application.Carts.Commands.CreateCartWithFirstProduct;
using Shopizy.Application.Carts.Commands.RemoveProductFromCart;
using Shopizy.Application.Carts.Commands.UpdateProductQuantity;
using Shopizy.Application.Carts.Queries.GetCart;
using Shopizy.Contracts.Cart;
using Shopizy.Contracts.Common;
using Swashbuckle.AspNetCore.Annotations;

namespace Shopizy.Api.Controllers;

[Route("api/v1.0/carts")]
public class CartController(ISender mediator, IMapper mapper) : ApiController
{
    private readonly ISender _mediator = mediator;
    private readonly IMapper _mapper = mapper;

    [HttpGet]
    [SwaggerResponse(StatusCodes.Status200OK, null, typeof(CartResponse))]
    [SwaggerResponse(StatusCodes.Status400BadRequest, null, typeof(ErrorResult))]
    [SwaggerResponse(StatusCodes.Status401Unauthorized, null, typeof(ErrorResult))]
    [SwaggerResponse(StatusCodes.Status500InternalServerError, null, typeof(ErrorResult))]
    public async Task<IActionResult> GetCartAsync(Guid userId)
    {
        var query = _mapper.Map<GetCartQuery>(userId);
        var result = await _mediator.Send(query);

        return result.Match(Product => Ok(_mapper.Map<CartResponse>(Product)), Problem);
    }

    [HttpPost]
    [SwaggerResponse(StatusCodes.Status200OK, null, typeof(CartResponse))]
    [SwaggerResponse(StatusCodes.Status400BadRequest, null, typeof(ErrorResult))]
    [SwaggerResponse(StatusCodes.Status401Unauthorized, null, typeof(ErrorResult))]
    [SwaggerResponse(StatusCodes.Status409Conflict, null, typeof(ErrorResult))]
    [SwaggerResponse(StatusCodes.Status500InternalServerError, null, typeof(ErrorResult))]
    public async Task<IActionResult> CreateCartWithFirstProductAsync(
        CreateCartWithFirstProductRequest request
    )
    {
        var command = _mapper.Map<CreateCartWithFirstProductCommand>(request);
        var result = await _mediator.Send(command);

        return result.Match(product => Ok(_mapper.Map<CartResponse>(product)), Problem);
    }

    [HttpPatch("{cartId:guid}")]
    [SwaggerResponse(StatusCodes.Status200OK, null, typeof(CartResponse))]
    [SwaggerResponse(StatusCodes.Status400BadRequest, null, typeof(ErrorResult))]
    [SwaggerResponse(StatusCodes.Status401Unauthorized, null, typeof(ErrorResult))]
    [SwaggerResponse(StatusCodes.Status409Conflict, null, typeof(ErrorResult))]
    [SwaggerResponse(StatusCodes.Status500InternalServerError, null, typeof(ErrorResult))]
    public async Task<IActionResult> AddProductToCartAsync(
        Guid cartId,
        AddProductToCartRequest request
    )
    {
        var command = _mapper.Map<AddProductToCartCommand>((cartId, request));
        var result = await _mediator.Send(command);

        return result.Match(product => Ok(_mapper.Map<CartResponse>(product)), Problem);
    }

    [HttpPatch("{cartId:guid}/items/{itemId:guid}")]
    [SwaggerResponse(StatusCodes.Status200OK, null, typeof(SuccessResult))]
    [SwaggerResponse(StatusCodes.Status400BadRequest, null, typeof(ErrorResult))]
    [SwaggerResponse(StatusCodes.Status401Unauthorized, null, typeof(ErrorResult))]
    [SwaggerResponse(StatusCodes.Status409Conflict, null, typeof(ErrorResult))]
    [SwaggerResponse(StatusCodes.Status500InternalServerError, null, typeof(ErrorResult))]
    public async Task<IActionResult> UpdateProductQuantityAsync(
        Guid cartId,
        Guid itemId,
        UpdateProductQuantityRequest request
    )
    {
        var command = _mapper.Map<UpdateProductQuantityCommand>((cartId, itemId, request));
        var result = await _mediator.Send(command);

        return result.Match(
            success => Ok(SuccessResult.Success("Successfully updated cart.")),
            Problem
        );
    }

    [HttpDelete("{cartId:guid}/items/{itemId:guid}")]
    [SwaggerResponse(StatusCodes.Status200OK, null, typeof(SuccessResult))]
    [SwaggerResponse(StatusCodes.Status400BadRequest, null, typeof(ErrorResult))]
    [SwaggerResponse(StatusCodes.Status401Unauthorized, null, typeof(ErrorResult))]
    [SwaggerResponse(StatusCodes.Status409Conflict, null, typeof(ErrorResult))]
    [SwaggerResponse(StatusCodes.Status500InternalServerError, null, typeof(ErrorResult))]
    public async Task<IActionResult> RemoveItemFromCartAsync(Guid cartId, Guid itemId)
    {
        var command = _mapper.Map<RemoveProductFromCartCommand>((cartId, itemId));
        var result = await _mediator.Send(command);

        return result.Match(
            success => Ok(SuccessResult.Success("Successfully removed product from cart.")),
            Problem
        );
    }
}
