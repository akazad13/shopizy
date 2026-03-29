namespace Shopizy.Application.Common.Constants;

public static class AppConstants
{
    public static class Pagination
    {
        public const int DefaultPageNumber = 1;
        public const int DefaultPageSize = 10;
        public const int MaxAdminReportCount = 100;
    }

    public static class DateRange
    {
        public const int MaxSalesReportDays = 90;
    }

    public static class Validation
    {
        public const int MaxEmailLength = 254;
        public const int MinPasswordLength = 8;
    }

    public static class Reporting
    {
        public const int DefaultTopProductsCount = 10;
        public const int LowStockThreshold = 5;
    }
}
