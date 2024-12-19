using ErrorOr;
using Shopizy.Application.Common.Interfaces.Services;
using Shopizy.Application.Common.models;
using Stripe;

namespace Shopizy.Infrastructure.ExternalServices.PaymentGateway.Stripe;

public class StripeService(
    CustomerService customerService,
    PaymentIntentService paymentIntentService
) : IPaymentService
{
    private readonly CustomerService _customerService = customerService;
    private readonly PaymentIntentService _paymentIntentService = paymentIntentService;

    public async Task<ErrorOr<CustomerResource>> CreateCustomer(
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

            return new CustomerResource(customer.Id, customer.Email, customer.Name);
        }
        catch (StripeException ex)
        {
            return Error.Failure(description: ex.Message);
        }
    }

    public async Task<ErrorOr<CreateSaleResponse>> CreateSaleAsync(CreateSaleRequest request)
    {
        var intentCreateOptions = new PaymentIntentCreateOptions
        {
            Customer = request.CustomerId,
            Amount = request.Amount,
            Currency = request.Currency,
            // ConfirmationMethod = "manual",  // if Confirm = false, then this will determine how a payment will be confirmed (From frontend/backend)
            Confirm = request.CapturePayment,
            PaymentMethodTypes = request.PaymentMethodTypes,
            Metadata = request.Metadata,
            PaymentMethod = request.PaymentMethodId,
        };

        try
        {
            var response = await _paymentIntentService.CreateAsync(intentCreateOptions);

            // var result = await _paymentIntentService.ConfirmAsync(response.Id); // if Confirm is false and we manaully confirm payment

            return new CreateSaleResponse
            {
                ResponseStatusCode = (int)response.StripeResponse.StatusCode,
                Amount = response.Amount,
                Currency = response.Currency,
                PaymentIntentId = response.Id,
                ObjectType = response.Object,
                PaymentMethodId = response.PaymentMethodId,
                CaptureMethod = response.CaptureMethod,
                CustomerId = response.CustomerId,
                ChargeId = response.LatestChargeId,
                Status = response.Status,
                Metadata = response.Metadata,
                PaymentMethodTypes = response.PaymentMethodTypes,
            };
        }
        catch (StripeException ex)
        {
            return Error.Failure(
                code: ex.StripeResponse.StatusCode.ToString(),
                description: FormatStripeException(ex)
            );
        }
        catch (Exception ex)
        {
            return Error.Failure(code: "500", description: ex.Message);
        }
    }

    private static string FormatStripeException(StripeException e)
    {
        return e.StripeError.Type switch
        {
            "card_error" => $"A payment error occurred: {e.StripeError.Message}",
            "api_connection_error" =>
                $"An error occurred while trying to connect to the stripe API: ${e.StripeError.Message}",
            "api_error" => $"An API error occurred: {e.StripeError.Message}",
            "authentication_error" =>
                $"An error occurred authenticating to Stripe API: {e.StripeError.Message}",
            "invalid_request_error" => $"An invalid request occurred: {e.StripeError.Message}",
            "rate_limit_error" => $"A rate limit error occurred: {e.StripeError.Message}",
            "validation_error" => $"A validation error occurred: {e.StripeError.Message}",
            _ => $"An unknown error occured: {e.StripeError.Message}",
        };
    }
}
