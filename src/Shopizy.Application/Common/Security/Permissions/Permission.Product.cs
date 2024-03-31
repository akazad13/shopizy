namespace Shopizy.Application.Common.Security.Permissions;

public static partial class Permission
{
    public static class Product
    {
        public const string Create = "create:product";
        public const string Get = "get:product";
        public const string Modify = "modify:product";
        public const string Delete = "delete:product";
    }
}
