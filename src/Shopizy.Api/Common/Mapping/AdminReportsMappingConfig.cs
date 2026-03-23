using Mapster;
using Shopizy.Application.Admin.Queries.GetSalesReport;
using Shopizy.Contracts.Admin;

namespace Shopizy.Api.Common.Mapping;

/// <summary>
/// Configures mapping for admin report-related models.
/// </summary>
public class AdminReportsMappingConfig : IRegister
{
    /// <summary>
    /// Registers the mapping configurations.
    /// </summary>
    /// <param name="config">The type adapter configuration.</param>
    public void Register(TypeAdapterConfig config)
    {
        ArgumentNullException.ThrowIfNull(config);

        config.NewConfig<TopProductDto, TopProductResponse>()
            .Map(dest => dest.Name, src => src.Name)
            .Map(dest => dest.TotalQuantity, src => src.TotalQuantity)
            .Map(dest => dest.Revenue, src => src.Revenue);

        config.NewConfig<TopCustomerDto, TopCustomerResponse>()
            .Map(dest => dest.UserId, src => src.UserId)
            .Map(dest => dest.FirstName, src => src.FirstName)
            .Map(dest => dest.LastName, src => src.LastName)
            .Map(dest => dest.TotalSpend, src => src.TotalSpend);

        config.NewConfig<SalesReportDto, SalesReportResponse>()
            .Map(dest => dest.StartDate, src => src.StartDate)
            .Map(dest => dest.EndDate, src => src.EndDate)
            .Map(dest => dest.TotalRevenue, src => src.TotalRevenue)
            .Map(dest => dest.TotalOrders, src => src.TotalOrders)
            .Map(dest => dest.TopProducts, src => src.TopProducts);
    }
}
