using ErrorOr;
using MapsterMapper;
using MediatR;
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

[Route("api/v1.0/users/{userId:guid}/orders")] 
public class OrderController(ISender mediator, IMapper mapper, ILogger<OrderController> logger)
    : ApiController
{
    private readonly ISender _mediator = mediator;
    private readonly IMapper _mapper = mapper;
    private readonly ILogger<OrderController> _logger = logger;

    [HttpGet]
    [SwaggerResponse(StatusCodes.Status200OK, null, typeof(List<OrderResponse>))]
    [SwaggerResponse(StatusCodes.Status400BadRequest, null, typeof(ErrorResult))]
    [SwaggerResponse(StatusCodes.Status401Unauthorized, null, typeof(ErrorResult))]
    [SwaggerResponse(StatusCodes.Status500InternalServerError, null, typeof(ErrorResult))]
    public async Task<IActionResult> GetOrdersAsync(Guid userId, [FromQuery] OrdersCriteria criteria)
    {
        try
        {
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

    [HttpGet("{orderId:guid}")]
    [SwaggerResponse(StatusCodes.Status200OK, null, typeof(OrderDetailResponse))]
    [SwaggerResponse(StatusCodes.Status400BadRequest, null, typeof(ErrorResult))]
    [SwaggerResponse(StatusCodes.Status401Unauthorized, null, typeof(ErrorResult))]
    [SwaggerResponse(StatusCodes.Status500InternalServerError, null, typeof(ErrorResult))]
    public async Task<IActionResult> GetOrderAsync(Guid userId, Guid orderId)
    {
        try
        {
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
