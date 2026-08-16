using Shopizy.SharedKernel.Domain.Models;

namespace Shopizy.Domain.Common.CustomErrors;

public static partial class CustomErrors
{
    public static class ReturnRequest
    {
        public static DomainError ReturnNotFound =>
            DomainError.NotFound(
                code: "ReturnRequest.ReturnNotFound",
                description: "Return request is not found."
            );

        public static DomainError ReturnNotPending =>
            DomainError.Validation(
                code: "ReturnRequest.ReturnNotPending",
                description: "Return request is not in a pending state."
            );

        public static DomainError ReturnAlreadyProcessed =>
            DomainError.Conflict(
                code: "ReturnRequest.ReturnAlreadyProcessed",
                description: "Return request has already been processed."
            );
    }
}
