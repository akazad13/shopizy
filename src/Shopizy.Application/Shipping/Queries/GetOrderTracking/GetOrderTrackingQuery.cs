using ErrorOr;
using Shopizy.Application.Common.Interfaces.Services;
using Shopizy.SharedKernel.Application.Messaging;

namespace Shopizy.Application.Shipping.Queries.GetOrderTracking;

public record GetOrderTrackingQuery(Guid OrderId) : IQuery<ErrorOr<ShippingTrackingInfoDto>>;
