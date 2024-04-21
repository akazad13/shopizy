namespace Shopizy.Application.Common.Security.Permissions;

public static partial class Permission
{
    public static class Order
    {
        public const string Create = "create:order";
        public const string Get = "get:order";
        public const string Modify = "modify:order";
        public const string Delete = "delete:order";
    }
}
