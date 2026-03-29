namespace Shopizy.Contracts.ProductQuestion;

public record ProductQuestionResponse(
    Guid QuestionId,
    string Question,
    bool IsAnswered,
    string? Answer,
    DateTime CreatedOn
);
