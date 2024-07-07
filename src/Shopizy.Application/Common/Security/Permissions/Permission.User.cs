namespace Shopizy.Application.Common.Security.Permissions;

public static partial class Permission
{
    public static class User
    {
        public const string Create = "create:user";
        public const string Get = "get:user";
        public const string Modify = "modify:user";
        public const string Delete = "delete:user";
    }
}
