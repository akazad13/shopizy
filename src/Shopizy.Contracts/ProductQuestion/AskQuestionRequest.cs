namespace Shopizy.Contracts.ProductQuestion;

/// <summary>
/// Submits a customer question against a product.
/// </summary>
/// <param name="Question">The question text. Visible publicly once answered.</param>
public record AskQuestionRequest(string Question);
