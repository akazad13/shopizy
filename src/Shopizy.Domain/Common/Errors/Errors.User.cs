using ErrorOr;

namespace Shopizy.Domain.Common.Errors;

public static partial class Errors
{
    public static class User
    {
        public static Error DuplicatePhone =>
            Error.Conflict(code: "User.DuplicatePhone", description: "Phone is already in use.");
        public static Error UserNotFound =>
            Error.NotFound(
                code: "User.UserNotFound",
                description: "User is not found with this phone & password."
            );
        public static Error UserNotCreated =>
            Error.Failure(code: "User.UserNotCreated", description: "Failed to create User.");
    }
}
