using ErrorOr;
using Shopizy.Application.Common.Interfaces.Services;
using Shopizy.Application.Common.models;
using Stripe;
using Stripe.Checkout;

namespace Shopizy.Infrastructure.ExternalServices.PaymentGateway.Stripe;

public class StripeService(
    TokenService tokenService,
    CustomerService customerService,
    ChargeService chargeService,
    SessionService sessionService
) : IPaymentService
{
    private readonly TokenService _tokenService = tokenService;
    private readonly CustomerService _customerService = customerService;
    private readonly ChargeService _chargeService = chargeService;
    private readonly SessionService _sessionService = sessionService;

    public async Task<ErrorOr<CustomerResource>> CreateCustomer(
        string email,
        string name,
        string number,
        string expiryYear,
        string expiryMonth,
        string cvc,
        CancellationToken cancellationToken
    )
    {
        try
        {
            var tokenOptions = new TokenCreateOptions
            {
                Card = new TokenCardOptions
                {
                    Name = name,
                    Number = number,
                    ExpYear = expiryYear,
                    ExpMonth = expiryMonth,
                    Cvc = cvc,
                },
            };
            Token token = await _tokenService.CreateAsync(tokenOptions, null, cancellationToken);

            var customerOptions = new CustomerCreateOptions
            {
                Email = email,
                Name = name,
                Source = token.Id,
            };
            Customer customer = await _customerService.CreateAsync(
                customerOptions,
                null,
                cancellationToken
            );

            return new CustomerResource(customer.Id, customer.Email, customer.Name);
        }
        catch (StripeException ex)
        {
            return Error.Failure(description: ex.Message);
        }
    }

    public async Task<ErrorOr<ChargeResource>> CreateCharge(
        string currency,
        long amount,
        string receiptEmail,
        string customerId,
        string description,
        CancellationToken cancellationToken
    )
    {
        try
        {
            var chargeOptions = new ChargeCreateOptions
            {
                Currency = currency,
                Amount = amount,
                ReceiptEmail = receiptEmail,
                Customer = customerId,
                Description = description,
            };

            Charge charge = await _chargeService.CreateAsync(chargeOptions, null, cancellationToken);

            return new ChargeResource(
                charge.Id,
                charge.Currency,
                charge.Amount,
                charge.CustomerId,
                charge.ReceiptEmail,
                charge.Description
            );
        }
        catch (StripeException ex)
        {
            return Error.Failure(description: ex.Message);
        }
    }

    public async Task<ErrorOr<CheckoutSession>> CreateCheckoutSession(
        string customerId,
        decimal price,
        string successUrl,
        string cancelUrl,
        CancellationToken cancellationToken = default
    )
    {
        try
        {
            var options = new SessionCreateOptions
            {
                //Customer = customerId,

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

            return new CheckoutSession(session.Id);
        }
        catch (StripeException ex)
        {
            return Error.Failure(description: ex.Message);
        }
    }
}
