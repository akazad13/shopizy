using Shopizy.Domain.Categories.ValueObjects;
using Shopizy.Domain.PromoCodes.Enums;
using Shopizy.Domain.PromoCodes.ValueObjects;
using Shopizy.SharedKernel.Domain.Models;

namespace Shopizy.Domain.PromoCodes;

/// <summary>
/// Represents a promotional code for discounts and advanced promotions.
/// </summary>
public sealed class PromoCode : AggregateRoot<PromoCodeId, Guid>, IAuditable
{
    /// <summary>
    /// Gets or sets the promo code string.
    /// </summary>
    public string Code { get; set; } = null!;

    /// <summary>
    /// Gets or sets the description of the promo code.
    /// </summary>
    public string Description { get; set; } = null!;

    /// <summary>
    /// Gets or sets the discount value.
    /// </summary>
    public decimal Discount { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the discount is a percentage.
    /// </summary>
    public bool IsPercentage { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the promo code is active.
    /// </summary>
    public bool IsActive { get; set; }

    /// <summary>
    /// Gets or sets the promotion type (Standard, Bogo, Tiered, CategorySpecific).
    /// </summary>
    public PromoType PromoType { get; set; } = PromoType.Standard;

    /// <summary>
    /// Gets or sets the minimum order subtotal required to use this promo code.
    /// </summary>
    public decimal? MinimumOrderAmount { get; set; }

    /// <summary>
    /// Gets or sets the maximum discount cap for percentage-based discounts.
    /// </summary>
    public decimal? MaxDiscountAmount { get; set; }

    /// <summary>
    /// Gets or sets the target category identifier for category-specific discounts.
    /// </summary>
    public CategoryId? TargetCategoryId { get; set; }

    /// <summary>
    /// Gets or sets the qualifying buy quantity for BOGO promotions.
    /// </summary>
    public int? BuyQuantity { get; set; }

    /// <summary>
    /// Gets or sets the reward get quantity for BOGO promotions.
    /// </summary>
    public int? GetQuantity { get; set; }

    /// <summary>
    /// Gets or sets the discount percentage applied to reward items in BOGO promotions (e.g. 100 for free, 50 for half off).
    /// </summary>
    public decimal? GetDiscountPercentage { get; set; }

    /// <summary>
    /// Gets or sets the lifetime usage limit for this promo code.
    /// </summary>
    public int? UsageLimit { get; set; }

    /// <summary>
    /// Gets or sets the start date for promo code validity.
    /// </summary>
    public DateTime? StartDate { get; set; }

    /// <summary>
    /// Gets or sets the expiration date for promo code validity.
    /// </summary>
    public DateTime? EndDate { get; set; }

    /// <summary>
    /// Gets or sets the number of times this promo code has been used.
    /// </summary>
    public int NumOfTimeUsed { get; set; }

    /// <summary>
    /// Gets or sets the date and time when the promo code was created.
    /// </summary>
    public DateTime CreatedOn { get; set; }

    /// <summary>
    /// Gets or sets the date and time when the promo code was last modified.
    /// </summary>
    public DateTime? ModifiedOn { get; set; }

    /// <summary>
    /// Creates a standard promo code.
    /// </summary>
    public static PromoCode Create(
        string code,
        string description,
        decimal discount,
        bool isPercentage,
        bool isActive
    ) =>
        new(
            PromoCodeId.CreateUnique(),
            code,
            description,
            discount,
            isPercentage,
            isActive,
            PromoType.Standard,
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            null
        );

    /// <summary>
    /// Creates a promo code with full advanced promotion configuration.
    /// </summary>
    public static PromoCode Create(
        string code,
        string description,
        decimal discount,
        bool isPercentage,
        bool isActive,
        PromoType promoType,
        decimal? minimumOrderAmount = null,
        decimal? maxDiscountAmount = null,
        CategoryId? targetCategoryId = null,
        int? buyQuantity = null,
        int? getQuantity = null,
        decimal? getDiscountPercentage = null,
        int? usageLimit = null,
        DateTime? startDate = null,
        DateTime? endDate = null
    ) =>
        new(
            PromoCodeId.CreateUnique(),
            code,
            description,
            discount,
            isPercentage,
            isActive,
            promoType,
            minimumOrderAmount,
            maxDiscountAmount,
            targetCategoryId,
            buyQuantity,
            getQuantity,
            getDiscountPercentage,
            usageLimit,
            startDate,
            endDate
        );

    public void Update(
        string code,
        string description,
        decimal discount,
        bool isPercentage,
        bool isActive,
        PromoType promoType = PromoType.Standard,
        decimal? minimumOrderAmount = null,
        decimal? maxDiscountAmount = null,
        CategoryId? targetCategoryId = null,
        int? buyQuantity = null,
        int? getQuantity = null,
        decimal? getDiscountPercentage = null,
        int? usageLimit = null,
        DateTime? startDate = null,
        DateTime? endDate = null
    )
    {
        Code = code;
        Description = description;
        Discount = discount;
        IsPercentage = isPercentage;
        IsActive = isActive;
        PromoType = promoType;
        MinimumOrderAmount = minimumOrderAmount;
        MaxDiscountAmount = maxDiscountAmount;
        TargetCategoryId = targetCategoryId;
        BuyQuantity = buyQuantity;
        GetQuantity = getQuantity;
        GetDiscountPercentage = getDiscountPercentage;
        UsageLimit = usageLimit;
        StartDate = startDate;
        EndDate = endDate;
    }

    /// <summary>
    /// Validates whether the promo code can be applied to the specified order subtotal at the given time.
    /// </summary>
    public bool IsValid(decimal orderSubtotal, DateTime currentUtc, out string? failureReason)
    {
        if (!IsActive)
        {
            failureReason = "Promo code is inactive.";
            return false;
        }

        if (StartDate.HasValue && currentUtc < StartDate.Value)
        {
            failureReason = "Promo code is not yet active.";
            return false;
        }

        if (EndDate.HasValue && currentUtc > EndDate.Value)
        {
            failureReason = "Promo code has expired.";
            return false;
        }

        if (UsageLimit.HasValue && NumOfTimeUsed >= UsageLimit.Value)
        {
            failureReason = "Promo code usage limit has been reached.";
            return false;
        }

        if (MinimumOrderAmount.HasValue && orderSubtotal < MinimumOrderAmount.Value)
        {
            failureReason = $"Minimum order amount of {MinimumOrderAmount.Value:C} required.";
            return false;
        }

        failureReason = null;
        return true;
    }

    /// <summary>
    /// Calculates the discount amount for a given order subtotal and items.
    /// </summary>
    public decimal CalculateDiscount(
        decimal orderSubtotal,
        IEnumerable<(CategoryId CategoryId, decimal UnitPrice, int Quantity)>? items = null
    )
    {
        if (orderSubtotal <= 0)
        {
            return 0;
        }

        decimal rawDiscount = 0;

        switch (PromoType)
        {
            case PromoType.CategorySpecific when TargetCategoryId is not null && items is not null:
                var categoryMatchingSubtotal = items
                    .Where(i => i.CategoryId == TargetCategoryId)
                    .Sum(i => i.UnitPrice * i.Quantity);

                if (categoryMatchingSubtotal <= 0)
                {
                    return 0;
                }

                rawDiscount = IsPercentage
                    ? categoryMatchingSubtotal * (Discount / 100m)
                    : Math.Min(Discount, categoryMatchingSubtotal);
                break;

            case PromoType.Bogo when BuyQuantity > 0 && GetQuantity > 0 && items is not null:
                var eligibleItems = (
                    TargetCategoryId is not null
                        ? items.Where(i => i.CategoryId == TargetCategoryId)
                        : items
                ).ToList();

                var totalUnits = eligibleItems.Sum(i => i.Quantity);
                var groupSize = BuyQuantity.Value + GetQuantity.Value;
                var groups = totalUnits / groupSize;
                var freeItemCount = groups * GetQuantity.Value;

                if (freeItemCount > 0)
                {
                    var unitPrices = eligibleItems
                        .SelectMany(i => Enumerable.Repeat(i.UnitPrice, i.Quantity))
                        .OrderBy(p => p)
                        .Take(freeItemCount)
                        .ToList();

                    var bogoPercentage = (GetDiscountPercentage ?? 100m) / 100m;
                    rawDiscount = unitPrices.Sum() * bogoPercentage;
                }
                break;

            case PromoType.Tiered:
            case PromoType.Standard:
            default:
                if (MinimumOrderAmount.HasValue && orderSubtotal < MinimumOrderAmount.Value)
                {
                    return 0;
                }

                rawDiscount = IsPercentage ? orderSubtotal * (Discount / 100m) : Discount;
                break;
        }

        if (MaxDiscountAmount.HasValue && rawDiscount > MaxDiscountAmount.Value)
        {
            rawDiscount = MaxDiscountAmount.Value;
        }

        return Math.Min(rawDiscount, orderSubtotal);
    }

    private PromoCode(
        PromoCodeId promoCodeId,
        string code,
        string description,
        decimal discount,
        bool isPercentage,
        bool isActive,
        PromoType promoType,
        decimal? minimumOrderAmount,
        decimal? maxDiscountAmount,
        CategoryId? targetCategoryId,
        int? buyQuantity,
        int? getQuantity,
        decimal? getDiscountPercentage,
        int? usageLimit,
        DateTime? startDate,
        DateTime? endDate
    )
        : base(promoCodeId)
    {
        Code = code;
        Description = description;
        Discount = discount;
        IsPercentage = isPercentage;
        IsActive = isActive;
        PromoType = promoType;
        MinimumOrderAmount = minimumOrderAmount;
        MaxDiscountAmount = maxDiscountAmount;
        TargetCategoryId = targetCategoryId;
        BuyQuantity = buyQuantity;
        GetQuantity = getQuantity;
        GetDiscountPercentage = getDiscountPercentage;
        UsageLimit = usageLimit;
        StartDate = startDate;
        EndDate = endDate;
        NumOfTimeUsed = 0;
    }

    private PromoCode() { }
}
