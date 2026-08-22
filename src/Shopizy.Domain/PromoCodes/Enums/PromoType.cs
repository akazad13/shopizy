namespace Shopizy.Domain.PromoCodes.Enums;

/// <summary>
/// Defines the promotion discount strategy.
/// </summary>
public enum PromoType
{
    /// <summary>
    /// Standard flat percentage or fixed amount discount off the whole order.
    /// </summary>
    Standard = 0,

    /// <summary>
    /// Buy X items, get Y items free or discounted (BOGO).
    /// </summary>
    Bogo = 1,

    /// <summary>
    /// Tiered discount applied when subtotal meets a minimum order threshold.
    /// </summary>
    Tiered = 2,

    /// <summary>
    /// Discount applies specifically to products in a designated category.
    /// </summary>
    CategorySpecific = 3,
}
