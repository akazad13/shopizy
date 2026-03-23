using Shopizy.Domain.ProductQuestions;
using Shopizy.Domain.ProductQuestions.ValueObjects;
using Shopizy.Domain.Products.ValueObjects;

namespace Shopizy.Application.Common.Interfaces.Persistence;

public interface IProductQuestionRepository
{
    Task<IReadOnlyList<ProductQuestion>> GetByProductIdAsync(ProductId productId);
    Task<ProductQuestion?> GetByIdAsync(ProductQuestionId id);
    Task AddAsync(ProductQuestion question);
    void Update(ProductQuestion question);
    void Remove(ProductQuestion question);
}
