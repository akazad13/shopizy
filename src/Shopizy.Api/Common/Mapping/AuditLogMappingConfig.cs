using Mapster;
using Shopizy.Contracts.AuditLog;
using Shopizy.Domain.AuditLogs;

namespace Shopizy.Api.Common.Mapping;

public class AuditLogMappingConfig : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        ArgumentNullException.ThrowIfNull(config);

        config
            .NewConfig<AuditLog, AuditLogResponse>()
            .Map(dest => dest.AuditLogId, src => src.Id.Value)
            .Map(dest => dest.UserId, src => src.UserId)
            .Map(dest => dest.Action, src => src.Action)
            .Map(dest => dest.EntityName, src => src.EntityName)
            .Map(dest => dest.EntityId, src => src.EntityId)
            .Map(dest => dest.OldValues, src => src.OldValues)
            .Map(dest => dest.NewValues, src => src.NewValues)
            .Map(dest => dest.Timestamp, src => src.Timestamp);
    }
}
