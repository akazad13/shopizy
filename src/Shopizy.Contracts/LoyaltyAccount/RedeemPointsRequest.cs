namespace Shopizy.Contracts.LoyaltyAccount;

/// <summary>
/// Redeems loyalty points from a user's account.
/// </summary>
/// <param name="Points">Points to redeem. Must be positive and not exceed the current balance.</param>
/// <param name="Description">Free-text reason for the debit (e.g., "order #42 discount").</param>
public record RedeemPointsRequest(int Points, string Description);
