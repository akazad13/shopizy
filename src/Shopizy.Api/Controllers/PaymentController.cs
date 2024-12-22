using MapsterMapper;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Shopizy.Application.Payments.Commands.CardNotPresentSale;
using Shopizy.Application.Payments.Commands.CashOnDeliverySale;
using Shopizy.Contracts.Common;
using Shopizy.Contracts.Payment;
using Swashbuckle.AspNetCore.Annotations;

namespace Shopizy.Api.Controllers;

[Route("api/v1.0/payments")]
public class PaymentController(ISender mediator, IMapper mapper) : ApiController
{
    private readonly ISender _mediator = mediator;
    private readonly IMapper _mapper = mapper;

    [HttpPost]
    [SwaggerResponse(StatusCodes.Status200OK, null, typeof(SuccessResult))]
    [SwaggerResponse(StatusCodes.Status400BadRequest, null, typeof(ErrorResult))]
    [SwaggerResponse(StatusCodes.Status401Unauthorized, null, typeof(ErrorResult))]
    [SwaggerResponse(StatusCodes.Status409Conflict, null, typeof(ErrorResult))]
    [SwaggerResponse(StatusCodes.Status500InternalServerError, null, typeof(ErrorResult))]
    public async Task<IActionResult> CreateSaleAsync(CardNotPresentSaleRequest request)
    {
        if (request.PaymentMethod.ToLower() == "card")
        {
            var command = _mapper.Map<CardNotPresentSaleCommand>(request);
            var result = await _mediator.Send(command);

            return result.Match(
                success => Ok(SuccessResult.Success("Payment successfull collected.")),
                Problem
            );
        }
        else
        {
            var command = _mapper.Map<CashOnDeliverySaleCommand>(request);
            var result = await _mediator.Send(command);

            return result.Match(
                success => Ok(SuccessResult.Success("Payment successfull collected.")),
                Problem
            );
        }
    }
}
