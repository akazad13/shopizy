using MapsterMapper;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Shopizy.Application.Orders.Commands.CancelOrder;
using Shopizy.Application.Orders.Commands.CreateOrder;
using Shopizy.Application.Orders.Queries.GetOrder;
using Shopizy.Application.Orders.Queries.GetOrders;
using Shopizy.Contracts.Common;
using Shopizy.Contracts.Order;
using Swashbuckle.AspNetCore.Annotations;

namespace Shopizy.Api.Controllers;

[Route("api/v1.0/orders")]
public class OrderController(ISender mediator, IMapper mapper) : ApiController
{
    private readonly ISender _mediator = mediator;
    private readonly IMapper _mapper = mapper;

    [HttpGet]
    [SwaggerResponse(StatusCodes.Status200OK, null, typeof(List<OrderResponse>))]
    [SwaggerResponse(StatusCodes.Status400BadRequest, null, typeof(ErrorResult))]
    [SwaggerResponse(StatusCodes.Status401Unauthorized, null, typeof(ErrorResult))]
    [SwaggerResponse(StatusCodes.Status500InternalServerError, null, typeof(ErrorResult))]
    public async Task<IActionResult> GetOrdersAsync([FromQuery] OrdersCriteria criteria)
    {
        var query = _mapper.Map<GetOrdersQuery>(criteria);
        var result = await _mediator.Send(query);

        return result.Match(Product => Ok(_mapper.Map<List<OrderResponse>>(Product)), Problem);
    }

    [HttpGet("{orderId:guid}")]
    [SwaggerResponse(StatusCodes.Status200OK, null, typeof(OrderDetailResponse))]
    [SwaggerResponse(StatusCodes.Status400BadRequest, null, typeof(ErrorResult))]
    [SwaggerResponse(StatusCodes.Status401Unauthorized, null, typeof(ErrorResult))]
    [SwaggerResponse(StatusCodes.Status500InternalServerError, null, typeof(ErrorResult))]
    public async Task<IActionResult> GetOrderAsync(Guid orderId)
    {
        var query = _mapper.Map<GetOrderQuery>(orderId);
        var result = await _mediator.Send(query);

        return result.Match(order => Ok(_mapper.Map<OrderDetailResponse>(order)), Problem);
    }

    [HttpPost]
    [SwaggerResponse(StatusCodes.Status200OK, null, typeof(OrderDetailResponse))]
    [SwaggerResponse(StatusCodes.Status400BadRequest, null, typeof(ErrorResult))]
    [SwaggerResponse(StatusCodes.Status401Unauthorized, null, typeof(ErrorResult))]
    [SwaggerResponse(StatusCodes.Status409Conflict, null, typeof(ErrorResult))]
    [SwaggerResponse(StatusCodes.Status500InternalServerError, null, typeof(ErrorResult))]
    public async Task<IActionResult> CreateOrderAsync(CreateOrderRequest request)
    {
        var command = _mapper.Map<CreateOrderCommand>(request);
        var result = await _mediator.Send(command);

        return result.Match(order => Ok(_mapper.Map<OrderDetailResponse>(order)), Problem);
    }

    [HttpDelete("{orderId:guid}")]
    [SwaggerResponse(StatusCodes.Status200OK, null, typeof(SuccessResult))]
    [SwaggerResponse(StatusCodes.Status400BadRequest, null, typeof(ErrorResult))]
    [SwaggerResponse(StatusCodes.Status401Unauthorized, null, typeof(ErrorResult))]
    [SwaggerResponse(StatusCodes.Status409Conflict, null, typeof(ErrorResult))]
    [SwaggerResponse(StatusCodes.Status500InternalServerError, null, typeof(ErrorResult))]
    public async Task<IActionResult> CancelOrderAsync(Guid orderId, CancelOrderRequest request)
    {
        var command = _mapper.Map<CancelOrderCommand>((orderId, request));
        var result = await _mediator.Send(command);

        return result.Match(
            success => Ok(SuccessResult.Success("Successfully canceled the order.")),
            Problem
        );
    }
}
