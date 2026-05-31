using Shopizy.Domain.LoyaltyAccounts.Enums;
using Shopizy.Domain.LoyaltyAccounts.ValueObjects;
using Shopizy.SharedKernel.Domain.Models;

namespace Shopizy.Domain.LoyaltyAccounts.Entities;

public sealed class LoyaltyTransaction : Entity<LoyaltyTransactionId>
{
    public int Points { get; }
    public LoyaltyTransactionType Type { get; }
    public string Description { get; } = null!;
    public DateTime CreatedOn { get; }

    public static LoyaltyTransaction Create(
        int points,
        LoyaltyTransactionType type,
        string description
    ) => new(LoyaltyTransactionId.CreateUnique(), points, type, description);

    private LoyaltyTransaction(
        LoyaltyTransactionId id,
        int points,
        LoyaltyTransactionType type,
        string description
    )
        : base(id)
    {
        Points = points;
        Type = type;
        Description = description;
        CreatedOn = DateTime.UtcNow;
    }

    private LoyaltyTransaction() { }
}
