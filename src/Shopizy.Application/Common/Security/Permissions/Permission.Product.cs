namespace Shopizy.Application.Common.Security.Permissions;

public static partial class Permission
{
    public static class Product
    {
        public const string Create = "create:Product";
        public const string Get = "get:Product";
        public const string Modify = "modify:Product";
        public const string Delete = "delete:Product";
    }
}
