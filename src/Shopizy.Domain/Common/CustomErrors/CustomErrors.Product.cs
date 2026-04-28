using Shopizy.SharedKernel.Domain.Models;

namespace Shopizy.Domain.Common.CustomErrors;

public static partial class CustomErrors
{
    public static class Product
    {
        public static DomainError DuplicateName =>
            DomainError.Conflict(
                code: "Product.DuplicateName",
                description: "Product with the same name is already in use."
            );
        public static DomainError ProductNotFound =>
            DomainError.NotFound(
                code: "Product.ProductNotFound",
                description: "Product is not found."
            );
        public static DomainError ProductNotCreated =>
            DomainError.Failure(
                code: "Product.ProductNotCreated",
                description: "Failed to create Product."
            );
        public static DomainError ProductNotUpdated =>
            DomainError.Failure(
                code: "Product.ProductNotUpdated",
                description: "Failed to update Product."
            );
        public static DomainError ProductNotDeleted =>
            DomainError.Failure(
                code: "Product.ProductNotDeleted",
                description: "Failed to delete Product."
            );
        public static DomainError ProductImageNotAdded =>
            DomainError.Failure(
                code: "Product.ProductImageNotAdded",
                description: "Failed to add Product image."
            );
        public static DomainError ProductImageNotUploaded =>
            DomainError.Failure(
                code: "Product.ProductImageNotUploaded",
                description: "Please upload a valid Product image."
            );
        public static DomainError ProductImageNotFound =>
            DomainError.Validation(
                code: "Product.ProductImageNotFound",
                description: "Product image is not found."
            );
        public static DomainError InsufficientStock =>
            DomainError.Validation(
                code: "Product.InsufficientStock",
                description: "Insufficient stock for the requested product quantity."
            );
    }
}
