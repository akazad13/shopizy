using Shopizy.SharedKernel.Domain.Models;

namespace Shopizy.Domain.Common.CustomErrors;

public static partial class CustomErrors
{
    public static class Brand
    {
        public static DomainError DuplicateName =>
            DomainError.Conflict(
                code: "Brand.DuplicateName",
                description: "Brand with the same name is already in use."
            );

        public static DomainError BrandNotFound =>
            DomainError.NotFound(code: "Brand.BrandNotFound", description: "Brand is not found.");

        public static DomainError BrandNotCreated =>
            DomainError.Failure(
                code: "Brand.BrandNotCreated",
                description: "Failed to create Brand."
            );

        public static DomainError BrandNotUpdated =>
            DomainError.Failure(
                code: "Brand.BrandNotUpdated",
                description: "Failed to update Brand."
            );

        public static DomainError BrandNotDeleted =>
            DomainError.Failure(
                code: "Brand.BrandNotDeleted",
                description: "Failed to delete Brand."
            );
    }
}
