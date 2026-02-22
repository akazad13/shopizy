using Shopizy.SharedKernel.Domain.Models;
using Shopizy.Domain.PromoCodes.ValueObjects;

namespace Shopizy.Domain.PromoCodes;

/// <summary>
/// Represents a promotional code for discounts.
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
    public bool IsPerchantage { get; set; }
    
    /// <summary>
    /// Gets or sets a value indicating whether the promo code is active.
    /// </summary>
    public bool IsActive { get; set; }
    
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
    /// Creates a new promo code.
    /// </summary>
    /// <param name="code">The promo code string.</param>
    /// <param name="description">The description.</param>
    /// <param name="discount">The discount value.</param>
    /// <param name="isPerchantage">Whether the discount is a percentage.</param>
    /// <param name="isActive">Whether the promo code is active.</param>
    /// <returns>A new <see cref="PromoCode"/> instance.</returns>
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
    }

    private PromoCode() { }
}
