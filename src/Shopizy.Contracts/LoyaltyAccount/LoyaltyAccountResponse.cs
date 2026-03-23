namespace Shopizy.Contracts.LoyaltyAccount;

public record LoyaltyAccountResponse(
    Guid AccountId,
    int TotalPoints,
    IReadOnlyList<LoyaltyTransactionResponse> Transactions
);

public record LoyaltyTransactionResponse(
    Guid TransactionId,
    int Points,
    string Type,
    string Description,
    DateTime CreatedOn
);
