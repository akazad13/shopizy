namespace Shopizy.Contracts.LoyaltyAccount;

/// <summary>
/// Adds loyalty points to a user's account.
/// </summary>
/// <param name="Points">Points to award. Must be positive.</param>
/// <param name="Description">Free-text reason for the credit (e.g., "order #42").</param>
public record EarnPointsRequest(int Points, string Description);
