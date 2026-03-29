using Shopizy.SharedKernel.Domain.Models;

namespace Shopizy.Domain.Common.CustomErrors;

public static partial class CustomErrors
{
    public static class Category
    {
        public static DomainError DuplicateName =>
            DomainError.Conflict(
                code: "Category.DuplicateName",
                description: "Category with the same name is already in use."
            );
        public static DomainError CategoryNotFound =>
            DomainError.NotFound(code: "Category.CategoryNotFound", description: "Category is not found.");
        public static DomainError CategoryNotCreated =>
            DomainError.Failure(
                code: "Category.CategoryNotCreated",
                description: "Failed to create Category."
            );
        public static DomainError CategoryNotUpdated =>
            DomainError.Failure(
                code: "Category.CategoryNotUpdated",
                description: "Failed to update Category."
            );
        public static DomainError CategoryNotDeleted =>
            DomainError.Failure(
                code: "Category.CategoryNotDeleted",
                description: "Failed to delete Category."
            );
    }
}
