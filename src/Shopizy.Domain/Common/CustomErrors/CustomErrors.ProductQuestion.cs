using ErrorOr;

namespace Shopizy.Domain.Common.CustomErrors;

public static partial class CustomErrors
{
    public static class ProductQuestion
    {
        public static Error QuestionNotFound =>
            Error.NotFound(code: "ProductQuestion.QuestionNotFound", description: "Product question is not found.");
        public static Error QuestionAlreadyAnswered =>
            Error.Conflict(code: "ProductQuestion.QuestionAlreadyAnswered", description: "This question has already been answered.");
        public static Error QuestionNotCreated =>
            Error.Failure(code: "ProductQuestion.QuestionNotCreated", description: "Failed to create product question.");
    }
}
