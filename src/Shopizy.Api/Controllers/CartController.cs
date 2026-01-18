using ErrorOr;
using MapsterMapper;
using MediatR;
using Microsoft.AspNetCore.Authorization;
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

/// <summary>
/// Controller for managing user shopping carts.
/// </summary>
[Authorize]
[Route("api/v1.0/users/{userId:guid}/carts")]
public class CartController(ISender mediator, IMapper mapper, ILogger<CartController> logger)
    : ApiController
{
    private readonly ISender _mediator = mediator;
    private readonly IMapper _mapper = mapper;
    private readonly ILogger<CartController> _logger = logger;

    /// <summary>
    /// Retrieves the shopping cart for a user.
    /// </summary>
    /// <param name="userId">The user identifier.</param>
    /// <returns>The user's shopping cart.</returns>
    /// <response code="200">Returns the cart.</response>
    /// <response code="400">If the request is invalid.</response>
    /// <response code="500">If an internal server error occurs.</response>
    [HttpGet]
    [SwaggerResponse(StatusCodes.Status200OK, null, typeof(CartResponse))]
    [SwaggerResponse(StatusCodes.Status400BadRequest, null, typeof(ErrorResult))]
    [SwaggerResponse(StatusCodes.Status401Unauthorized, null, typeof(ErrorResult))]
    [SwaggerResponse(StatusCodes.Status500InternalServerError, null, typeof(ErrorResult))]
    public async Task<IActionResult> GetCartAsync(Guid userId)
    {
        try
        {
            if (!IsAuthorized(userId)) return Problem([Error.Forbidden(description: "You are not authorized to access this cart.")]);

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

    /// <summary>
    /// Adds a product to the user's cart.
    /// </summary>
    /// <param name="userId">The user identifier.</param>
    /// <param name="cartId">The cart identifier.</param>
    /// <param name="request">The request containing product details.</param>
    /// <returns>The updated cart.</returns>
    /// <response code="200">Returns the updated cart.</response>
    /// <response code="400">If the request is invalid.</response>
    /// <response code="409">If there is a conflict.</response>
    /// <response code="500">If an internal server error occurs.</response>
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
            if (!IsAuthorized(userId)) return Problem([Error.Forbidden(description: "You are not authorized to modify this cart.")]);

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

    /// <summary>
    /// Updates the quantity of a product in the cart.
    /// </summary>
    /// <param name="userId">The user identifier.</param>
    /// <param name="cartId">The cart identifier.</param>
    /// <param name="itemId">The cart item identifier.</param>
    /// <param name="request">The request containing the new quantity.</param>
    /// <returns>Success result.</returns>
    /// <response code="200">If update is successful.</response>
    /// <response code="400">If the request is invalid.</response>
    /// <response code="500">If an internal server error occurs.</response>
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
            if (!IsAuthorized(userId)) return Problem([Error.Forbidden(description: "You are not authorized to update this cart.")]);

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

    /// <summary>
    /// Removes an item from the cart.
    /// </summary>
    /// <param name="userId">The user identifier.</param>
    /// <param name="cartId">The cart identifier.</param>
    /// <param name="itemId">The cart item identifier.</param>
    /// <returns>Success result.</returns>
    /// <response code="200">If removal is successful.</response>
    /// <response code="400">If the request is invalid.</response>
    /// <response code="500">If an internal server error occurs.</response>
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
            if (!IsAuthorized(userId)) return Problem([Error.Forbidden(description: "You are not authorized to remove items from this cart.")]);

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
