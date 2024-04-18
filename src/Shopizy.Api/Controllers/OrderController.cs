using MapsterMapper;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Shopizy.Application.Carts.Commands.CreateCartWithFirstProduct;
using Shopizy.Contracts.Cart;
using Shopizy.Contracts.Order;

namespace Shopizy.Api.Controllers;

[Route("api/users/{userId:guid}/orders")]
public class OrderController(ISender _mediator, IMapper _mapper) : ApiController
{
    // [HttpGet("{orderId:guid}")]
    // public async Task<IActionResult> GetOrder(Guid userId, Guid orderId)
    // {
    //     var query = _mapper.Map<GetCartQuery>(userId);
    //     var result = await _mediator.Send(query);

    //     return result.Match(
    //         Product => Ok(_mapper.Map<CartResponse?>(Product)),
    //         Problem);
    // }

    [HttpPost]
    public async Task<IActionResult> CreateOrder(Guid userId, CreateOrderRequest request)
    {
        var command = _mapper.Map<CreateCartWithFirstProductCommand>((userId, request));
        var result = await _mediator.Send(command);

        return result.Match(
            product => Ok(_mapper.Map<CartResponse>(product)),
            Problem);
    }

    // [HttpDelete("{orderId:guid}")]
    // public async Task<IActionResult> CancelOrder(Guid userId, Guid orderId, RemoveProductFromCartRequest request)
    // {
    //     var command = _mapper.Map<RemoveProductFromCartCommand>((userId, orderId, request));
    //     var result = await _mediator.Send(command);

    //     return result.Match(
    //         success => Ok(success),
    //         Problem);
    // }
}