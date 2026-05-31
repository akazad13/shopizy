using Shopizy.Domain.ProductQuestions.ValueObjects;
using Shopizy.Domain.Users.ValueObjects;
using Shopizy.SharedKernel.Domain.Models;

namespace Shopizy.Domain.ProductQuestions.Entities;

public sealed class ProductAnswer : Entity<ProductAnswerId>
{
    public UserId AnsweredByUserId { get; } = null!;
    public string Answer { get; } = null!;
    public DateTime CreatedOn { get; }

    public static ProductAnswer Create(UserId answeredByUserId, string answer) =>
        new(ProductAnswerId.CreateUnique(), answeredByUserId, answer);

    private ProductAnswer(ProductAnswerId id, UserId answeredByUserId, string answer)
        : base(id)
    {
        AnsweredByUserId = answeredByUserId;
        Answer = answer;
        CreatedOn = DateTime.UtcNow;
    }

    private ProductAnswer() { }
}
