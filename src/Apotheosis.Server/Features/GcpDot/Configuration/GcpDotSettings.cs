namespace Apotheosis.Server.Features.GcpDot.Configuration;

public sealed class GcpDotSettings
{
    public const string Name = "GcpDot";

    /// <summary>
    /// The base URL for GCP Dot requests.
    /// </summary>
    public Uri? BaseUrl { get; set; }
}