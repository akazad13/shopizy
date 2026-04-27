namespace Shopizy.Contracts.ProductQuestion;

/// <summary>
/// A product Q&amp;A entry.
/// </summary>
/// <param name="QuestionId">Identifier of the question.</param>
/// <param name="Question">The question text.</param>
/// <param name="IsAnswered">True once an answer has been posted.</param>
/// <param name="Answer">The answer text, when present.</param>
/// <param name="CreatedOn">UTC timestamp when the question was submitted.</param>
public record ProductQuestionResponse(
    Guid QuestionId,
    string Question,
    bool IsAnswered,
    string? Answer,
    DateTime CreatedOn
);
