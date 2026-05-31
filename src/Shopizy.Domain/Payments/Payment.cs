using Shopizy.Domain.Common.Enums;
using Shopizy.Domain.Common.ValueObjects;
using Shopizy.Domain.Orders;
using Shopizy.Domain.Orders.ValueObjects;
using Shopizy.Domain.Payments.Enums;
using Shopizy.Domain.Payments.ValueObjects;
using Shopizy.Domain.Users;
using Shopizy.Domain.Users.ValueObjects;
using Shopizy.SharedKernel.Domain.Models;

namespace Shopizy.Domain.Payments;

/// <summary>
/// Represents a payment transaction in the system.
/// </summary>
public sealed class Payment : Entity<PaymentId>, IAuditable
{
    /// <summary>
    /// Gets or sets the order associated with this payment.
    /// </summary>
    public Order? Order { get; private set; }

    /// <summary>
    /// Gets or sets the order identifier.
    /// </summary>
    public required OrderId OrderId { get; init; }

    /// <summary>
    /// Gets or sets the user who made the payment.
    /// </summary>
    public User? User { get; private set; }

    /// <summary>
    /// Gets or sets the user identifier.
    /// </summary>
    public required UserId UserId { get; init; }

    /// <summary>
    /// Gets the payment method type.
    /// </summary>
    public required string PaymentMethod { get; init; }

    /// <summary>
    /// Gets the payment method identifier from the payment provider.
    /// </summary>
    public required string PaymentMethodId { get; init; }

    /// <summary>
    /// Gets the transaction identifier.
    /// </summary>
    public string TransactionId { get; private set; } = string.Empty;

    /// <summary>
    /// Gets the current payment status.
    /// </summary>
    public PaymentStatus PaymentStatus { get; private set; }

    /// <summary>
    /// Gets the total payment amount.
    /// </summary>
    public required Price Total { get; init; }

    /// <summary>
    /// Gets the billing address.
    /// </summary>
    public required Address BillingAddress { get; init; }

    /// <summary>
    /// Gets the date and time when the payment was created.
    /// </summary>
    public DateTime CreatedOn { get; }

    /// <summary>
    /// Gets the date and time when the payment was last modified.
    /// </summary>
    public DateTime? ModifiedOn { get; private set; }

    /// <summary>
    /// Creates a new payment.
    /// </summary>
    /// <param name="userId">The user identifier.</param>
    /// <param name="orderId">The order identifier.</param>
    /// <param name="paymentMethod">The payment method.</param>
    /// <param name="paymentMethodId">The payment method identifier.</param>
    /// <param name="transactionId">The transaction identifier.</param>
    /// <param name="paymentStatus">The payment status.</param>
    /// <param name="total">The total amount.</param>
    /// <param name="billingAddress">The billing address.</param>
    /// <returns>A new <see cref="Payment"/> instance.</returns>
    public static Payment Create(
        UserId userId,
        OrderId orderId,
        string paymentMethod,
        string paymentMethodId,
        string transactionId,
        PaymentStatus paymentStatus,
        Price total,
        Address billingAddress
    )
    {
        var payment = new Payment
        {
            Id = PaymentId.CreateUnique(),
            UserId = userId,
            OrderId = orderId,
            PaymentMethod = paymentMethod,
            PaymentMethodId = paymentMethodId,
            TransactionId = transactionId,
            PaymentStatus = paymentStatus,
            Total = total,
            BillingAddress = billingAddress,
        };

        return payment;
    }

    private Payment() { }

    /// <summary>
    /// Marks this payment as successfully completed, records the charge ID,
    /// and raises <see cref="Events.PaymentSucceededDomainEvent"/> so the associated
    /// order can be transitioned to Processing via a domain event handler.
    /// </summary>
    /// <param name="chargeId"></param>
    /// <param name="customerId"></param>
    public void Complete(string chargeId, string customerId)
    {
        PaymentStatus = PaymentStatus.Payed;
        TransactionId = chargeId;
        AddDomainEvent(new Events.PaymentSucceededDomainEvent(OrderId, customerId));
    }

    /// <summary>
    /// Updates the payment status.
    /// </summary>
    /// <param name="status">The new payment status.</param>
    public void UpdatePaymentStatus(PaymentStatus status) => PaymentStatus = status;

    /// <summary>
    /// Updates the transaction identifier.
    /// </summary>
    /// <param name="transactionId">The new transaction identifier.</param>
    public void UpdateTransactionId(string transactionId) => TransactionId = transactionId;
}
