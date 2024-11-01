using Microsoft.Extensions.Options;
using Shopizy.Application.Common.Interfaces.Services;
using Shopizy.Application.Common.models;
using Shopizy.Application.Common.Wrappers;
using Stripe;
using Stripe.Checkout;

namespace Shopizy.Infrastructure.ExternalServices.PaymentGateway.Stripe;

public class StripeService(
    CustomerService customerService,
    SessionService sessionService,
    IOptions<StripeSettings> options
) : IPaymentService
{
    private readonly CustomerService _customerService = customerService;
    private readonly SessionService _sessionService = sessionService;
    private readonly StripeSettings _stripeSettings = options.Value;

    public async Task<IResult<CustomerResource>> CreateCustomer(
        string email,
        string name,
        CancellationToken cancellationToken
    )
    {
        try
        {
            var customerOptions = new CustomerCreateOptions { Email = email, Name = name };
            Customer customer = await _customerService.CreateAsync(
                options: customerOptions,
                requestOptions: null,
                cancellationToken: cancellationToken
            );

            return Response<CustomerResource>.SuccessResponese(
                new CustomerResource(customer.Id, customer.Email, customer.Name)
            );
        }
        catch (StripeException ex)
        {
            return Response<CustomerResource>.ErrorResponse([ex.Message]);
        }
    }

    public async Task<IResult<CheckoutSession>> CreateCheckoutSession(
        string customerEmail,
        decimal price,
        string successUrl,
        string cancelUrl,
        CancellationToken cancellationToken = default
    )
    {
        // var searchOptions = new CustomerSearchOptions { Query = $"email:'{customerEmail}'" };
        // var customer = await _customerService.SearchAsync(searchOptions, null, cancellationToken);

        try
        {
            var options = new SessionCreateOptions
            {
                Customer = customerEmail,
                LineItems =
                [
                    new SessionLineItemOptions
                    {
                        PriceData = new SessionLineItemPriceDataOptions
                        {
                            ProductData = new SessionLineItemPriceDataProductDataOptions
                            {
                                Name = "test Name",
                                Description = "test description",
                                Images =
                                [
                                    "https://st2.depositphotos.com/2251265/8722/i/950/depositphotos_87226702-stock-photo-bearded-young-man-standing-on.jpg",
                                ],
                            },
                            UnitAmountDecimal = price * 100,
                            Currency = "usd",
                        },
                        Quantity = 1,
                    },
                ],
                PaymentMethodTypes = ["card"],
                Mode = "payment",
                SuccessUrl = successUrl,
                CancelUrl = cancelUrl,
                AllowPromotionCodes = true,
            };

            Session session = await _sessionService.CreateAsync(options, null, cancellationToken);

            return Response<CheckoutSession>.SuccessResponese(
                new CheckoutSession(session.Id, _stripeSettings.PublishableKey)
            );
        }
        catch (StripeException ex)
        {
            return Response<CheckoutSession>.ErrorResponse([ex.Message]);
        }
    }
}
