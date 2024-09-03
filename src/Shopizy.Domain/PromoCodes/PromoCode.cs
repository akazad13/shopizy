using Shopizy.Domain.Common.Models;
using Shopizy.Domain.PromoCodes.ValueObjects;

namespace Shopizy.Domain.PromoCodes;

public sealed class PromoCode : AggregateRoot<PromoCodeId, Guid>
{
    public string Code { get; set; }
    public string Description { get; set; }
    public decimal Discount { get; set; }
    public bool IsPerchantage { get; set; }
    public bool IsActive { get; set; }
    public int NumOfTimeUsed { get; set; }
    public DateTime CreatedOn { get; set; }
    public DateTime? ModifiedOn { get; set; }

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
            isActive
        );
    }

    private PromoCode(
        PromoCodeId promoCodeId,
        string code,
        string description,
        decimal discount,
        bool isPerchantage,
        bool isActive
    ) : base(promoCodeId)
    {
        Code = code;
        Description = description;
        Discount = discount;
        IsPerchantage = isPerchantage;
        IsActive = isActive;
        NumOfTimeUsed = 0;
        CreatedOn = DateTime.UtcNow;
    }

    private PromoCode() { }
}
