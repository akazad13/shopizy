using ErrorOr;
using Shopizy.SharedKernel.Application.Messaging;

namespace Shopizy.Application.Admin.Queries.ExportAnalytics;

public record AnalyticsExportFile(byte[] Content, string ContentType, string FileName);

public record ExportAnalyticsQuery(string Format = "csv") : IQuery<ErrorOr<AnalyticsExportFile>>;
