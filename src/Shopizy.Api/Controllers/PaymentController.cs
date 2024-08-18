using MapsterMapper;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Shopizy.Application.Payments.Commands.CreatePayment;
using Shopizy.Contracts.Payment;

namespace Shopizy.Api.Controllers;

[Route("api/v1.0/users/{userId:guid}/Payments")]
public class PaymentController(ISender _mediator, IMapper _mapper) : ApiController
{
    [HttpPost]
    public async Task<IActionResult> CreatePayment(Guid userId, CreatePaymentRequest request)
    {
        var command = _mapper.Map<CreatePaymentCommand>((userId, request));
        var result = await _mediator.Send(command);

        return result.Match(Payment => Ok(_mapper.Map<PaymentResponse>(Payment)), Problem);
    }
}
