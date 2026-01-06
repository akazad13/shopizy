using ErrorOr;
using MapsterMapper;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shopizy.Api.Common.LoggerMessages;
using Shopizy.Application.Payments.Commands.CardNotPresentSale;
using Shopizy.Application.Payments.Commands.CashOnDeliverySale;
using Shopizy.Contracts.Common;
using Shopizy.Contracts.Payment;
using Swashbuckle.AspNetCore.Annotations;

namespace Shopizy.Api.Controllers;

/// <summary>
/// Controller for processing payments.
/// </summary>
[Authorize]
[Route("api/v1.0/users/{userId:guid}/payments")]
public class PaymentController(ISender mediator, IMapper mapper, ILogger<PaymentController> logger)
    : ApiController
{
    private readonly ISender _mediator = mediator;
    private readonly IMapper _mapper = mapper;
    private readonly ILogger<PaymentController> _logger = logger;

    /// <summary>
    /// Processes a payment for a user.
    /// </summary>
    /// <param name="userId">The user identifier.</param>
    /// <param name="request">The payment request details.</param>
    /// <returns>Success result if payment is collected.</returns>
    /// <response code="200">If payment is successful.</response>
    /// <response code="400">If the request is invalid.</response>
    /// <response code="500">If an internal server error occurs.</response>
    [HttpPost]
    [SwaggerResponse(StatusCodes.Status200OK, null, typeof(SuccessResult))]
    [SwaggerResponse(StatusCodes.Status400BadRequest, null, typeof(ErrorResult))]
    [SwaggerResponse(StatusCodes.Status401Unauthorized, null, typeof(ErrorResult))]
    [SwaggerResponse(StatusCodes.Status409Conflict, null, typeof(ErrorResult))]
    [SwaggerResponse(StatusCodes.Status500InternalServerError, null, typeof(ErrorResult))]
    public async Task<IActionResult> PayAsync(Guid userId, CardNotPresentSaleRequest request)
    {
        try
        {
            if (request.PaymentMethod.ToLower() == "card")
            {
                var command = _mapper.Map<CardNotPresentSaleCommand>((userId, request));
                var result = await _mediator.Send(command);

                return result.Match(
                    success => Ok(SuccessResult.Success("Payment successfull collected.")),
                    Problem
                );
            }
            else
            {
                var command = _mapper.Map<CashOnDeliverySaleCommand>((userId, request));
                var result = await _mediator.Send(command);

                return result.Match(
                    success => Ok(SuccessResult.Success("Payment successfull collected.")),
                    Problem
                );
            }
        }
        catch (Exception ex)
        {
            _logger.PaymentError(ex);
            return Problem([Error.Unexpected(description: ex.Message)]);
        }
    }
}
