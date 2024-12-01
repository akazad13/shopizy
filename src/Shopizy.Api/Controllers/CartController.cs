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

[Route("api/v1.0/users/{userId:guid}/carts")]
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
        Guid userId,
        CreateCartWithFirstProductRequest request
    )
    {
        var command = _mapper.Map<CreateCartWithFirstProductCommand>((userId, request));
        var result = await _mediator.Send(command);

        return result.Match(product => Ok(_mapper.Map<CartResponse>(product)), Problem);
    }

    [HttpPatch("{cartId:guid}/add-product")]
    [SwaggerResponse(StatusCodes.Status200OK, null, typeof(CartResponse))]
    [SwaggerResponse(StatusCodes.Status400BadRequest, null, typeof(ErrorResult))]
    [SwaggerResponse(StatusCodes.Status401Unauthorized, null, typeof(ErrorResult))]
    [SwaggerResponse(StatusCodes.Status409Conflict, null, typeof(ErrorResult))]
    [SwaggerResponse(StatusCodes.Status500InternalServerError, null, typeof(ErrorResult))]
    public async Task<IActionResult> AddProductToCartAsync(
        Guid userId,
        Guid cartId,
        AddProductToCartRequest request
    )
    {
        var command = _mapper.Map<AddProductToCartCommand>((userId, cartId, request));
        var result = await _mediator.Send(command);

        return result.Match(product => Ok(_mapper.Map<CartResponse>(product)), Problem);
    }

    [HttpPatch("{cartId:guid}/update-quantity")]
    [SwaggerResponse(StatusCodes.Status200OK, null, typeof(SuccessResult))]
    [SwaggerResponse(StatusCodes.Status400BadRequest, null, typeof(ErrorResult))]
    [SwaggerResponse(StatusCodes.Status401Unauthorized, null, typeof(ErrorResult))]
    [SwaggerResponse(StatusCodes.Status409Conflict, null, typeof(ErrorResult))]
    [SwaggerResponse(StatusCodes.Status500InternalServerError, null, typeof(ErrorResult))]
    public async Task<IActionResult> UpdateProductQuantityAsync(
        Guid userId,
        Guid cartId,
        UpdateProductQuantityRequest request
    )
    {
        var command = _mapper.Map<UpdateProductQuantityCommand>((userId, cartId, request));
        var result = await _mediator.Send(command);

        return result.Match(
            success => Ok(SuccessResult.Success("Successfully updated cart.")),
            Problem
        );
    }

    [HttpDelete("{cartId:guid}/product/{productId:guid}")]
    [SwaggerResponse(StatusCodes.Status200OK, null, typeof(SuccessResult))]
    [SwaggerResponse(StatusCodes.Status400BadRequest, null, typeof(ErrorResult))]
    [SwaggerResponse(StatusCodes.Status401Unauthorized, null, typeof(ErrorResult))]
    [SwaggerResponse(StatusCodes.Status409Conflict, null, typeof(ErrorResult))]
    [SwaggerResponse(StatusCodes.Status500InternalServerError, null, typeof(ErrorResult))]
    public async Task<IActionResult> RemoveProductFromCartAsync(
        Guid userId,
        Guid cartId,
        Guid productId
    )
    {
        var command = _mapper.Map<RemoveProductFromCartCommand>((userId, cartId, productId));
        var result = await _mediator.Send(command);

        return result.Match(
            success => Ok(SuccessResult.Success("Successfully removed product from cart.")),
            Problem
        );
    }
}
