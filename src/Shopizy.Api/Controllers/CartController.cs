using ErrorOr;
using MapsterMapper;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Shopizy.Api.Common.LoggerMessages;
using Shopizy.Application.Carts.Commands.AddProductToCart;
using Shopizy.Application.Carts.Commands.RemoveProductFromCart;
using Shopizy.Application.Carts.Commands.UpdateProductQuantity;
using Shopizy.Application.Carts.Queries.GetCart;
using Shopizy.Contracts.Cart;
using Shopizy.Contracts.Common;
using Swashbuckle.AspNetCore.Annotations;

namespace Shopizy.Api.Controllers;

[Route("api/v1.0/users/{userId:guid}/carts")]
public class CartController(ISender mediator, IMapper mapper, ILogger<CartController> logger)
    : ApiController
{
    private readonly ISender _mediator = mediator;
    private readonly IMapper _mapper = mapper;
    private readonly ILogger<CartController> _logger = logger;

    [HttpGet]
    [SwaggerResponse(StatusCodes.Status200OK, null, typeof(CartResponse))]
    [SwaggerResponse(StatusCodes.Status400BadRequest, null, typeof(ErrorResult))]
    [SwaggerResponse(StatusCodes.Status401Unauthorized, null, typeof(ErrorResult))]
    [SwaggerResponse(StatusCodes.Status500InternalServerError, null, typeof(ErrorResult))]
    public async Task<IActionResult> GetCartAsync(Guid userId)
    {
        try
        {
            var query = _mapper.Map<GetCartQuery>(userId);
            var result = await _mediator.Send(query);

            return result.Match(Product => Ok(_mapper.Map<CartResponse>(Product)), Problem);
        }
        catch (Exception ex)
        {
            _logger.CartFetchError(ex);
            return Problem([Error.Unexpected(description: ex.Message)]);
        }
    }

    [HttpPatch("{cartId:guid}")]
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
        try
        {
            var command = _mapper.Map<AddProductToCartCommand>((userId, cartId, request));
            var result = await _mediator.Send(command);

            return result.Match(product => Ok(_mapper.Map<CartResponse>(product)), Problem);
        }
        catch (Exception ex)
        {
            _logger.CartCreationError(ex);
            return Problem([Error.Unexpected(description: ex.Message)]);
        }
    }

    [HttpPatch("{cartId:guid}/items/{itemId:guid}")]
    [SwaggerResponse(StatusCodes.Status200OK, null, typeof(SuccessResult))]
    [SwaggerResponse(StatusCodes.Status400BadRequest, null, typeof(ErrorResult))]
    [SwaggerResponse(StatusCodes.Status401Unauthorized, null, typeof(ErrorResult))]
    [SwaggerResponse(StatusCodes.Status409Conflict, null, typeof(ErrorResult))]
    [SwaggerResponse(StatusCodes.Status500InternalServerError, null, typeof(ErrorResult))]
    public async Task<IActionResult> UpdateProductQuantityAsync(
        Guid userId,
        Guid cartId,
        Guid itemId,
        UpdateProductQuantityRequest request
    )
    {
        try
        {
            var command = _mapper.Map<UpdateProductQuantityCommand>((userId, cartId, itemId, request));
            var result = await _mediator.Send(command);

            return result.Match(
                success => Ok(SuccessResult.Success("Successfully updated cart.")),
                Problem
            );
        }
        catch (Exception ex)
        {
            _logger.CartUpdateError(ex);
            return Problem([Error.Unexpected(description: ex.Message)]);
        }
    }

    [HttpDelete("{cartId:guid}/items/{itemId:guid}")]
    [SwaggerResponse(StatusCodes.Status200OK, null, typeof(SuccessResult))]
    [SwaggerResponse(StatusCodes.Status400BadRequest, null, typeof(ErrorResult))]
    [SwaggerResponse(StatusCodes.Status401Unauthorized, null, typeof(ErrorResult))]
    [SwaggerResponse(StatusCodes.Status409Conflict, null, typeof(ErrorResult))]
    [SwaggerResponse(StatusCodes.Status500InternalServerError, null, typeof(ErrorResult))]
    public async Task<IActionResult> RemoveItemFromCartAsync(Guid userId, Guid cartId, Guid itemId)
    {
        try
        {
            var command = _mapper.Map<RemoveProductFromCartCommand>((userId, cartId, itemId));
            var result = await _mediator.Send(command);

            return result.Match(
                success => Ok(SuccessResult.Success("Successfully removed product from cart.")),
                Problem
            );
        }
        catch (Exception ex)
        {
            _logger.RemoveItemFromCartError(ex);
            return Problem([Error.Unexpected(description: ex.Message)]);
        }
    }
}
