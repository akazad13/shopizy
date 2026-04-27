namespace Shopizy.Contracts.ProductQuestion;

/// <summary>
/// Posts an answer to a product question.
/// </summary>
/// <param name="Answer">Answer text. Visible publicly.</param>
public record AnswerQuestionRequest(string Answer);
