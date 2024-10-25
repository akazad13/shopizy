using MapsterMapper;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Shopizy.Application.Common.Wrappers;
using Shopizy.Application.Orders.Commands.CancelOrder;
using Shopizy.Application.Orders.Commands.CreateOrder;
using Shopizy.Application.Orders.Queries.GetOrder;
using Shopizy.Application.Orders.Queries.ListOrders;
using Shopizy.Contracts.Order;
using Swashbuckle.AspNetCore.Annotations;

namespace Shopizy.Api.Controllers;

[Route("api/v1.0/users/{userId:guid}/orders")]
public class OrderController(ISender mediator, IMapper mapper) : ApiController
{
    private readonly ISender _mediator = mediator;
    private readonly IMapper _mapper = mapper;

    [HttpGet]
    [SwaggerResponse(StatusCodes.Status200OK, null, typeof(List<OrderResponse>))]
    [SwaggerResponse(StatusCodes.Status400BadRequest, null, typeof(GenericResponse))]
    [SwaggerResponse(StatusCodes.Status401Unauthorized, null, typeof(GenericResponse))]
    [SwaggerResponse(StatusCodes.Status500InternalServerError, null, typeof(GenericResponse))]
    public async Task<IActionResult> GetOrdersAsync(Guid UserId)
    {
        var query = _mapper.Map<ListOrdersQuery>(UserId);
        var result = await _mediator.Send(query);

        return result.Match<IActionResult>(
            Product => Ok(_mapper.Map<List<OrderResponse>>(Product)),
            BadRequest
        );
    }

    [HttpGet("{orderId:guid}")]
    [SwaggerResponse(StatusCodes.Status200OK, null, typeof(OrderResponse))]
    [SwaggerResponse(StatusCodes.Status400BadRequest, null, typeof(GenericResponse))]
    [SwaggerResponse(StatusCodes.Status401Unauthorized, null, typeof(GenericResponse))]
    [SwaggerResponse(StatusCodes.Status500InternalServerError, null, typeof(GenericResponse))]
    public async Task<IActionResult> GetOrderAsync(Guid userId, Guid orderId)
    {
        var query = _mapper.Map<GetOrderQuery>((userId, orderId));
        var result = await _mediator.Send(query);

        return result.Match<IActionResult>(
            order => Ok(_mapper.Map<OrderResponse?>(order)),
            BadRequest
        );
    }

    [HttpPost]
    [SwaggerResponse(StatusCodes.Status200OK, null, typeof(OrderResponse))]
    [SwaggerResponse(StatusCodes.Status400BadRequest, null, typeof(GenericResponse))]
    [SwaggerResponse(StatusCodes.Status401Unauthorized, null, typeof(GenericResponse))]
    [SwaggerResponse(StatusCodes.Status409Conflict, null, typeof(GenericResponse))]
    [SwaggerResponse(StatusCodes.Status500InternalServerError, null, typeof(GenericResponse))]
    public async Task<IActionResult> CreateOrderAsync(Guid userId, CreateOrderRequest request)
    {
        var command = _mapper.Map<CreateOrderCommand>((userId, request));
        var result = await _mediator.Send(command);

        return result.Match<IActionResult>(
            order => Ok(_mapper.Map<OrderResponse>(order)),
            BadRequest
        );
    }

    [HttpDelete("{orderId:guid}")]
    [SwaggerResponse(StatusCodes.Status200OK, null, typeof(GenericResponse))]
    [SwaggerResponse(StatusCodes.Status400BadRequest, null, typeof(GenericResponse))]
    [SwaggerResponse(StatusCodes.Status401Unauthorized, null, typeof(GenericResponse))]
    [SwaggerResponse(StatusCodes.Status409Conflict, null, typeof(GenericResponse))]
    [SwaggerResponse(StatusCodes.Status500InternalServerError, null, typeof(GenericResponse))]
    public async Task<IActionResult> CancelOrderAsync(
        Guid userId,
        Guid orderId,
        CancelOrderRequest request
    )
    {
        var command = _mapper.Map<CancelOrderCommand>((userId, orderId, request));
        var result = await _mediator.Send(command);

        return result.Match<IActionResult>(success => Ok(success), BadRequest);
    }
}
