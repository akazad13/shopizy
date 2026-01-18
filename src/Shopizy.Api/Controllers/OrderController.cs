using ErrorOr;
using MapsterMapper;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shopizy.Api.Common.LoggerMessages;
using Shopizy.Application.Orders.Commands.CancelOrder;
using Shopizy.Application.Orders.Commands.CreateOrder;
using Shopizy.Application.Orders.Queries.GetOrder;
using Shopizy.Application.Orders.Queries.GetOrders;
using Shopizy.Contracts.Common;
using Shopizy.Contracts.Order;
using Swashbuckle.AspNetCore.Annotations;

namespace Shopizy.Api.Controllers;

/// <summary>
/// Controller for managing user orders.
/// </summary>
[Authorize]
[Route("api/v1.0/users/{userId:guid}/orders")] 
public class OrderController(ISender mediator, IMapper mapper, ILogger<OrderController> logger)
    : ApiController
{
    private readonly ISender _mediator = mediator;
    private readonly IMapper _mapper = mapper;
    private readonly ILogger<OrderController> _logger = logger;

    /// <summary>
    /// Retrieves a list of orders for a user based on criteria.
    /// </summary>
    /// <param name="userId">The user identifier.</param>
    /// <param name="criteria">The order search criteria.</param>
    /// <returns>A list of orders.</returns>
    /// <response code="200">Returns the list of orders.</response>
    /// <response code="400">If the request is invalid.</response>
    /// <response code="500">If an internal server error occurs.</response>
    [HttpGet]
    [SwaggerResponse(StatusCodes.Status200OK, null, typeof(List<OrderResponse>))]
    [SwaggerResponse(StatusCodes.Status400BadRequest, null, typeof(ErrorResult))]
    [SwaggerResponse(StatusCodes.Status401Unauthorized, null, typeof(ErrorResult))]
    [SwaggerResponse(StatusCodes.Status500InternalServerError, null, typeof(ErrorResult))]
    public async Task<IActionResult> GetOrdersAsync(Guid userId, [FromQuery] OrdersCriteria criteria)
    {
        try
        {
            if (!IsAuthorized(userId)) return Problem([Error.Forbidden(description: "You are not authorized to view these orders.")]);

            var query = _mapper.Map<GetOrdersQuery>((userId, criteria));
            var result = await _mediator.Send(query);

            return result.Match(Product => Ok(_mapper.Map<List<OrderResponse>>(Product)), Problem);
        }
        catch (Exception ex)
        {
            _logger.OrderFetchError(ex);
            return Problem([Error.Unexpected(description: ex.Message)]);
        }
    }

    /// <summary>
    /// Retrieves a specific order by its identifier.
    /// </summary>
    /// <param name="userId">The user identifier.</param>
    /// <param name="orderId">The order identifier.</param>
    /// <returns>The requested order details.</returns>
    /// <response code="200">Returns the order details.</response>
    /// <response code="400">If the request is invalid.</response>
    /// <response code="500">If an internal server error occurs.</response>
    [HttpGet("{orderId:guid}")]
    [SwaggerResponse(StatusCodes.Status200OK, null, typeof(OrderDetailResponse))]
    [SwaggerResponse(StatusCodes.Status400BadRequest, null, typeof(ErrorResult))]
    [SwaggerResponse(StatusCodes.Status401Unauthorized, null, typeof(ErrorResult))]
    [SwaggerResponse(StatusCodes.Status500InternalServerError, null, typeof(ErrorResult))]
    public async Task<IActionResult> GetOrderAsync(Guid userId, Guid orderId)
    {
        try
        {
            if (!IsAuthorized(userId)) return Problem([Error.Forbidden(description: "You are not authorized to view this order.")]);

            var query = _mapper.Map<GetOrderQuery>((userId, orderId));
            var result = await _mediator.Send(query);

            return result.Match(order => Ok(_mapper.Map<OrderDetailResponse>(order)), Problem);
        }
        catch (Exception ex)
        {
            _logger.OrderFetchError(ex);
            return Problem([Error.Unexpected(description: ex.Message)]);
        }
    }

    /// <summary>
    /// Creates a new order.
    /// </summary>
    /// <param name="userId">The user identifier.</param>
    /// <param name="request">The order creation request.</param>
    /// <returns>The created order details.</returns>
    /// <response code="200">Returns the created order.</response>
    /// <response code="400">If the request is invalid.</response>
    /// <response code="409">If there is a conflict.</response>
    /// <response code="500">If an internal server error occurs.</response>
    [HttpPost]
    [SwaggerResponse(StatusCodes.Status200OK, null, typeof(OrderDetailResponse))]
    [SwaggerResponse(StatusCodes.Status400BadRequest, null, typeof(ErrorResult))]
    [SwaggerResponse(StatusCodes.Status401Unauthorized, null, typeof(ErrorResult))]
    [SwaggerResponse(StatusCodes.Status409Conflict, null, typeof(ErrorResult))]
    [SwaggerResponse(StatusCodes.Status500InternalServerError, null, typeof(ErrorResult))]
    public async Task<IActionResult> CreateOrderAsync(Guid userId, CreateOrderRequest request)
    {
        try
        {
            if (!IsAuthorized(userId)) return Problem([Error.Forbidden(description: "You are not authorized to create an order for this user.")]);

            var command = _mapper.Map<CreateOrderCommand>((userId, request));
            var result = await _mediator.Send(command);

            return result.Match(order => Ok(_mapper.Map<OrderDetailResponse>(order)), Problem);
        }
        catch (Exception ex)
        {
            _logger.OrderFetchError(ex);
            return Problem([Error.Unexpected(description: ex.Message)]);
        }
    }

    /// <summary>
    /// Cancels an existing order.
    /// </summary>
    /// <param name="userId">The user identifier.</param>
    /// <param name="orderId">The order identifier.</param>
    /// <param name="request">The cancel order request.</param>
    /// <returns>Success result.</returns>
    /// <response code="200">If cancellation is successful.</response>
    /// <response code="400">If the request is invalid.</response>
    /// <response code="500">If an internal server error occurs.</response>
    [HttpPatch("{orderId:guid}/cancel")]
    [SwaggerResponse(StatusCodes.Status200OK, null, typeof(SuccessResult))]
    [SwaggerResponse(StatusCodes.Status400BadRequest, null, typeof(ErrorResult))]
    [SwaggerResponse(StatusCodes.Status401Unauthorized, null, typeof(ErrorResult))]
    [SwaggerResponse(StatusCodes.Status409Conflict, null, typeof(ErrorResult))]
    [SwaggerResponse(StatusCodes.Status500InternalServerError, null, typeof(ErrorResult))]
    public async Task<IActionResult> CancelOrderAsync(Guid userId, Guid orderId, CancelOrderRequest request)
    {
        try
        {
            if (!IsAuthorized(userId)) return Problem([Error.Forbidden(description: "You are not authorized to cancel this order.")]);

            var command = _mapper.Map<CancelOrderCommand>((userId, orderId, request));
            var result = await _mediator.Send(command);

            return result.Match(
                success => Ok(SuccessResult.Success("Successfully canceled the order.")),
                Problem
            );
        }
        catch (Exception ex)
        {
            _logger.OrderFetchError(ex);
            return Problem([Error.Unexpected(description: ex.Message)]);
        }
    }
}
