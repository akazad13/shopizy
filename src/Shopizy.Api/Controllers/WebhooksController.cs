using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Shopizy.Infrastructure.ExternalServices.PaymentGateway.Stripe;
using Stripe;

namespace Shopizy.Api.Controllers;

[Route("api/v1.0/webhooks")]
public class Webhooks(IOptions<StripeSettings> options) : ApiController
{
    private readonly StripeSettings _stripeSettings = options.Value;

    [HttpPost("stripe")]
    public async Task<IActionResult> StripeWebhookAsync()
    {
        string json = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync();
        Event stripeEvent;
        try
        {
            string webhookSecret = _stripeSettings.WebhookSecret;
            stripeEvent = EventUtility.ConstructEvent(
                json,
                Request.Headers["Stripe-Signature"].ToString(),
                webhookSecret
            );
            Console.WriteLine(
                $"Webhook notification with type: {stripeEvent.Type} found for {stripeEvent.Id}"
            );



            if (stripeEvent.Type == Events.PaymentIntentSucceeded)
            {
                // Some code here
            }
            else if (stripeEvent.Type == Events.CustomerCreated)
            {
                if (stripeEvent.Data.Object is Customer)
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
        }

        catch (Exception e)
        {
            Console.WriteLine($"Something failed {e}");
            return BadRequest();
        }

        return Ok();
    }
}
