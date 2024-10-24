using MapsterMapper;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Shopizy.Application.Payments.Commands.CreatePaymentSession;
using Shopizy.Contracts.Payment;

namespace Shopizy.Api.Controllers;

[Route("api/v1.0/users/{userId:guid}/payments")]
public class PaymentController(ISender mediator, IMapper mapper) : ApiController
{
    private readonly ISender _mediator = mediator;
    private readonly IMapper _mapper = mapper;

    [HttpPost("session")]
    public async Task<IActionResult> CreatePaymentSessoinAsync(
        Guid userId,
        CreatePaymentSessionRequest request
    )
    {
        CreatePaymentSessionCommand command = _mapper.Map<CreatePaymentSessionCommand>(
            (userId, request)
        );
        ErrorOr.ErrorOr<Application.Common.models.CheckoutSession> result = await _mediator.Send(
            command
        );

        return result.Match(Payment => Ok(_mapper.Map<PaymentSessionResponse>(Payment)), Problem);
    }
}
