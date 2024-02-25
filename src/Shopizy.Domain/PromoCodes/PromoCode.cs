using Shopizy.Domain.Common.Models;
using shopizy.Domain.PromoCodes.ValueObjects;

namespace shopizy.Domain.PromoCodes;

public sealed class PromoCode : AggregateRoot<PromoCodeId, Guid>
{
    public string Code { get; set; }
    public string Description { get; set; }
    public decimal Discount { get; set; }
    public bool IsPerchantage { get; set; }
    public bool IsActive { get; set; }
    public int NumOfTimeUsed { get; set; }
    public DateTime CreatedOn { get; set; }
    public DateTime ModifiedOn { get; set; }

    public static PromoCode Create(
        string code,
        string description,
        decimal discount,
        bool isPerchantage,
        bool isActive
    )
    {
        return new PromoCode(
            PromoCodeId.CreateUnique(),
            code,
            description,
            discount,
            isPerchantage,
            isActive,
            0,
            DateTime.UtcNow,
            DateTime.UtcNow
        );
    }

    private PromoCode(
        PromoCodeId promoCodeId,
        string code,
        string description,
        decimal discount,
        bool isPerchantage,
        bool isActive,
        int numOfTimeUsed,
        DateTime createdOn,
        DateTime modifiedOn
    ) : base(promoCodeId)
    {
        Code = code;
        Description = description;
        Discount = discount;
        IsPerchantage = isPerchantage;
        IsActive = isActive;
        NumOfTimeUsed = numOfTimeUsed;
        CreatedOn = createdOn;
        ModifiedOn = modifiedOn;
    }

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    private PromoCode() { }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
}
