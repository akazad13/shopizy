using MapsterMapper;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Shopizy.Application.Orders.Commands.CancelOrder;
using Shopizy.Application.Orders.Commands.CreateOrder;
using Shopizy.Application.Orders.Queries.GetOrder;
using Shopizy.Application.Orders.Queries.ListOrders;
using Shopizy.Contracts.Order;

namespace Shopizy.Api.Controllers;

[Route("api/v1.0/users/{userId:guid}/orders")]
public class OrderController(ISender _mediator, IMapper _mapper) : ApiController
{
    [HttpGet]
    public async Task<IActionResult> GetOrders(Guid UserId)
    {
        var query = _mapper.Map<ListOrdersQuery>(UserId);
        var result = await _mediator.Send(query);

        return result.Match(Product => Ok(_mapper.Map<List<OrderResponse>>(Product)), Problem);
    }

    [HttpGet("{orderId:guid}")]
    public async Task<IActionResult> GetOrder(Guid userId, Guid orderId)
    {
        var query = _mapper.Map<GetOrderQuery>((userId, orderId));
        var result = await _mediator.Send(query);

        return result.Match(order => Ok(_mapper.Map<OrderResponse?>(order)), Problem);
    }

    [HttpPost]
    public async Task<IActionResult> CreateOrder(Guid userId, CreateOrderRequest request)
    {
        var command = _mapper.Map<CreateOrderCommand>((userId, request));
        var result = await _mediator.Send(command);

        return result.Match(order => Ok(_mapper.Map<OrderResponse>(order)), Problem);
    }

    [HttpDelete("{orderId:guid}")]
    public async Task<IActionResult> CancelOrder(
        Guid userId,
        Guid orderId,
        CancelOrderRequest request
    )
    {
        var command = _mapper.Map<CancelOrderCommand>((userId, orderId, request));
        var result = await _mediator.Send(command);

        return result.Match(success => Ok(success), Problem);
    }
}
