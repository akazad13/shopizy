using System.Diagnostics.CodeAnalysis;

namespace Shopizy.Infrastructure.ExternalServices.MediaUploader.CloudinaryService;

[ExcludeFromCodeCoverage]
public class CloudinarySettings
{
    public const string Section = "CloudinarySettings";
    public string CloudName { get; set; } = string.Empty;
    public string ApiKey { get; set; } = string.Empty;
    public string ApiSecret { get; set; } = string.Empty;
    public bool Secure { get; set; }
}
