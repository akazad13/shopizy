using MapsterMapper;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Stripe;

namespace Shopizy.Api.Controllers;

[Route("api/v1.0/webhooks")]
public class Webhooks(ISender _mediator, IMapper _mapper) : ApiController
{
    [HttpPost("stripe-webhook")]
    public async Task<IActionResult> StripeWebhook()
    {
        var json = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync();
        Event stripeEvent;
        try
        {
            var webhookSecret = "StripeWebhookSecret";
            stripeEvent = EventUtility.ConstructEvent(
                json,
                Request.Headers["Stripe-Signature"].ToString(),
                webhookSecret
            );
            Console.WriteLine(
                $"Webhook notification with type: {stripeEvent.Type} found for {stripeEvent.Id}"
            );
        }
        catch (Exception e)
        {
            Console.WriteLine($"Something failed {e}");
            return BadRequest();
        }

        if (stripeEvent.Type == Events.PaymentIntentSucceeded)
        {
            // Some code here
        }
        else if (stripeEvent.Type == Events.CustomerCreated)
        {
            if (stripeEvent.Data.Object is Customer customer)
            {
                // var result = await _accountService.UpdateStripeCustomerId(
                //     customer.Email,
                //     customer.Id
                // );
                // if (!result)
                // {
                //     Console.WriteLine($"Failed to add stripe customer id {customer.Id} to user");
                // }
            }
        }
        else if (stripeEvent.Type == Events.CheckoutSessionCompleted) { }
        else if (stripeEvent.Type == Events.InvoicePaid) { }
        else if (stripeEvent.Type == Events.InvoicePaymentFailed) { }
        else
        {
            Console.WriteLine("Default");
        }

        return Ok();
    }
}
