namespace Shopizy.Domain.Common.CustomErrors;

public static partial class CustomErrors
{
    public static class User
    {
        public static string DuplicatePhone => "Phone is already in use.";
        public static string UserNotFoundWhileLogin =>
            "User is not found with this phone & password.";
        public static string UserNotFound => "User is not found.";
        public static string UserNotCreated => "Failed to create User.";
        public static string UserNotUpdated => "Failed to update.";
        public static string PasswordNotCorrect => "Password is not correct.";
        public static string PasswordNotUpdated => "Failed to update password.";
    }
}
