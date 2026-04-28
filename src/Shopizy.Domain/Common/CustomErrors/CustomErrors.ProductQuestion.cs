using Shopizy.SharedKernel.Domain.Models;

namespace Shopizy.Domain.Common.CustomErrors;

public static partial class CustomErrors
{
    public static class ProductQuestion
    {
        public static DomainError QuestionNotFound =>
            DomainError.NotFound(
                code: "ProductQuestion.QuestionNotFound",
                description: "Product question is not found."
            );
        public static DomainError QuestionAlreadyAnswered =>
            DomainError.Conflict(
                code: "ProductQuestion.QuestionAlreadyAnswered",
                description: "This question has already been answered."
            );
        public static DomainError QuestionNotCreated =>
            DomainError.Failure(
                code: "ProductQuestion.QuestionNotCreated",
                description: "Failed to create product question."
            );
    }
}
