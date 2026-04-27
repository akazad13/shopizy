namespace Shopizy.Contracts.LoyaltyAccount;

/// <summary>
/// A user's loyalty account with current balance and transaction history.
/// </summary>
/// <param name="AccountId">Identifier of the loyalty account.</param>
/// <param name="TotalPoints">Current redeemable points balance.</param>
/// <param name="Transactions">Append-only history of point movements (newest last).</param>
public record LoyaltyAccountResponse(
    Guid AccountId,
    int TotalPoints,
    IReadOnlyList<LoyaltyTransactionResponse> Transactions
);

/// <summary>
/// A single loyalty point movement.
/// </summary>
/// <param name="TransactionId">Identifier of the transaction.</param>
/// <param name="Points">Signed delta: positive for earn, negative for redeem.</param>
/// <param name="Type">Transaction type (Earn, Redeem, Expire, Adjust).</param>
/// <param name="Description">Free-text reason recorded at the time of the movement.</param>
/// <param name="CreatedOn">UTC timestamp when the movement was recorded.</param>
public record LoyaltyTransactionResponse(
    Guid TransactionId,
    int Points,
    string Type,
    string Description,
    DateTime CreatedOn
);
