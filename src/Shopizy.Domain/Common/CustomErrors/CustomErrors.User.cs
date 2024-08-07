using ErrorOr;

namespace Shopizy.Domain.Common.CustomErrors;

public static partial class CustomErrors
{
    public static class User
    {
        public static Error DuplicatePhone =>
            Error.Conflict(code: "User.DuplicatePhone", description: "Phone is already in use.");
        public static Error UserNotFoundWhileLogin =>
            Error.NotFound(
                code: "User.UserNotFound",
                description: "User is not found with this phone & password."
            );
        public static Error UserNotFound =>
            Error.NotFound(code: "User.UserNotFound", description: "User is not found.");
        public static Error UserNotCreated =>
            Error.Failure(code: "User.UserNotCreated", description: "Failed to create User.");
        public static Error UserNotUpdated =>
            Error.NotFound(code: "User.UserNotUpdated", description: "Failed to update.");
        public static Error PasswordNotCorrect =>
            Error.NotFound(
                code: "User.PasswordNotCorrect",
                description: "Password is not correct."
            );
        public static Error PasswordNotUpdated =>
            Error.NotFound(
                code: "User.PasswordNotUpdated",
                description: "Failed to update password."
            );
    }
}
