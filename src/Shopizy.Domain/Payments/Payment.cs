using Shopizy.Domain.Common.Models;
using Shopizy.Domain.Common.ValueObjects;
using Shopizy.Domain.Orders;
using Shopizy.Domain.Orders.ValueObjects;
using Shopizy.Domain.Payments.Enums;
using Shopizy.Domain.Payments.ValueObjects;
using Shopizy.Domain.Users;
using Shopizy.Domain.Users.ValueObjects;

namespace Shopizy.Domain.Payments;

/// <summary>
/// Represents a payment transaction in the system.
/// </summary>
public sealed class Payment : Entity<PaymentId>
{
    /// <summary>
    /// Gets or sets the order associated with this payment.
    /// </summary>
    public Order Order { get; set; } = null!;
    
    /// <summary>
    /// Gets or sets the order identifier.
    /// </summary>
    public OrderId OrderId { get; set; }
    
    /// <summary>
    /// Gets or sets the user who made the payment.
    /// </summary>
    public User User { get; set; } = null!;
    
    /// <summary>
    /// Gets or sets the user identifier.
    /// </summary>
    public UserId UserId { get; set; }
    
    /// <summary>
    /// Gets the payment method type.
    /// </summary>
    public string PaymentMethod { get; private set; }
    
    /// <summary>
    /// Gets the payment method identifier from the payment provider.
    /// </summary>
    public string PaymentMethodId { get; private set; }
    
    /// <summary>
    /// Gets the transaction identifier.
    /// </summary>
    public string TransactionId { get; private set; }
    
    /// <summary>
    /// Gets the current payment status.
    /// </summary>
    public PaymentStatus PaymentStatus { get; private set; }
    
    /// <summary>
    /// Gets the total payment amount.
    /// </summary>
    public Price Total { get; private set; }
    
    /// <summary>
    /// Gets the billing address.
    /// </summary>
    public Address BillingAddress { get; }
    
    /// <summary>
    /// Gets the date and time when the payment was created.
    /// </summary>
    public DateTime CreatedOn { get; private set; }
    
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
        return new Payment(
            PaymentId.CreateUnique(),
            userId,
            orderId,
            paymentMethod,
            paymentMethodId,
            transactionId,
            paymentStatus,
            total,
            billingAddress
        );
    }

    private Payment(
        PaymentId PaymentId,
        UserId userId,
        OrderId orderId,
        string paymentMethod,
        string paymentMethodId,
        string transactionId,
        PaymentStatus paymentStatus,
        Price total,
        Address billingAddress
    )
        : base(PaymentId)
    {
        UserId = userId;
        OrderId = orderId;
        PaymentMethod = paymentMethod;
        PaymentMethodId = paymentMethodId;
        TransactionId = transactionId;
        PaymentStatus = paymentStatus;
        Total = total;
        BillingAddress = billingAddress;
        CreatedOn = DateTime.UtcNow;
    }

    private Payment() { }

    /// <summary>
    /// Updates the payment status.
    /// </summary>
    /// <param name="status">The new payment status.</param>
    public void UpdatePaymentStatus(PaymentStatus status)
    {
        PaymentStatus = status;
        ModifiedOn = DateTime.UtcNow;
    }

    /// <summary>
    /// Updates the transaction identifier.
    /// </summary>
    /// <param name="transactionId">The new transaction identifier.</param>
    public void UpdateTransactionId(string transactionId)
    {
        TransactionId = transactionId;
        ModifiedOn = DateTime.UtcNow;
    }
}
