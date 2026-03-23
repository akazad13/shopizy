using ErrorOr;
using Shopizy.Domain.Common.CustomErrors;
using Shopizy.Domain.GiftCards.ValueObjects;
using Shopizy.Domain.Users.ValueObjects;
using Shopizy.SharedKernel.Domain.Models;

namespace Shopizy.Domain.GiftCards;

public sealed class GiftCard : AggregateRoot<GiftCardId, Guid>, IAuditable
{
    public string Code { get; private set; } = null!;
    public decimal InitialBalance { get; private set; }
    public decimal RemainingBalance { get; private set; }
    public bool IsActive { get; private set; }
    public DateTime? ExpiresOn { get; private set; }
    public UserId? RedeemedByUserId { get; private set; }
    public DateTime? RedeemedOn { get; private set; }
    public DateTime CreatedOn { get; private set; }
    public DateTime? ModifiedOn { get; private set; }

    public static GiftCard Create(string code, decimal initialBalance, DateTime? expiresOn)
    {
        return new GiftCard(GiftCardId.CreateUnique(), code, initialBalance, expiresOn);
    }

    public ErrorOr<Updated> Redeem(UserId userId)
    {
        if (!IsActive)
        {
            return CustomErrors.GiftCard.GiftCardInactive;
        }

        if (RedeemedByUserId is not null)
        {
            return CustomErrors.GiftCard.GiftCardAlreadyRedeemed;
        }

        if (ExpiresOn.HasValue && ExpiresOn.Value < DateTime.UtcNow)
        {
            return CustomErrors.GiftCard.GiftCardExpired;
        }

        if (RemainingBalance <= 0)
        {
            return CustomErrors.GiftCard.GiftCardInactive;
        }

        RedeemedByUserId = userId;
        RedeemedOn = DateTime.UtcNow;
        IsActive = false;
        ModifiedOn = DateTime.UtcNow;

        return Result.Updated;
    }

    public void Deactivate()
    {
        IsActive = false;
        ModifiedOn = DateTime.UtcNow;
    }

    private GiftCard(GiftCardId id, string code, decimal initialBalance, DateTime? expiresOn) : base(id)
    {
        Code = code;
        InitialBalance = initialBalance;
        RemainingBalance = initialBalance;
        IsActive = true;
        ExpiresOn = expiresOn;
        CreatedOn = DateTime.UtcNow;
    }

    private GiftCard() { }
}
