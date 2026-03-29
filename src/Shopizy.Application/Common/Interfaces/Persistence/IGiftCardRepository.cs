using Shopizy.Domain.GiftCards;
using Shopizy.Domain.GiftCards.ValueObjects;

namespace Shopizy.Application.Common.Interfaces.Persistence;

public interface IGiftCardRepository
{
    Task<IReadOnlyList<GiftCard>> GetAllAsync(int pageNumber, int pageSize);
    Task<GiftCard?> GetByIdAsync(GiftCardId id);
    Task<GiftCard?> GetByCodeAsync(string code);
    Task AddAsync(GiftCard giftCard);
    void Update(GiftCard giftCard);
    void Remove(GiftCard giftCard);
}
