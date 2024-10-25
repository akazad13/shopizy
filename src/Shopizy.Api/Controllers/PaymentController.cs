using MapsterMapper;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Shopizy.Application.Common.Wrappers;
using Shopizy.Application.Payments.Commands.CreatePaymentSession;
using Shopizy.Contracts.Payment;
using Swashbuckle.AspNetCore.Annotations;

namespace Shopizy.Api.Controllers;

[Route("api/v1.0/users/{userId:guid}/payments")]
public class PaymentController(ISender mediator, IMapper mapper) : ApiController
{
    private readonly ISender _mediator = mediator;
    private readonly IMapper _mapper = mapper;

    [HttpPost("session")]
    [SwaggerResponse(StatusCodes.Status200OK, null, typeof(PaymentSessionResponse))]
    [SwaggerResponse(StatusCodes.Status400BadRequest, null, typeof(GenericResponse))]
    [SwaggerResponse(StatusCodes.Status401Unauthorized, null, typeof(GenericResponse))]
    [SwaggerResponse(StatusCodes.Status409Conflict, null, typeof(GenericResponse))]
    [SwaggerResponse(StatusCodes.Status500InternalServerError, null, typeof(GenericResponse))]
    public async Task<IActionResult> CreatePaymentSessoinAsync(
        Guid userId,
        CreatePaymentSessionRequest request
    )
    {
        var command = _mapper.Map<CreatePaymentSessionCommand>((userId, request));
        var result = await _mediator.Send(command);

        return result.Match<IActionResult>(
            Payment => Ok(_mapper.Map<PaymentSessionResponse>(Payment)),
            BadRequest
        );
    }
}
