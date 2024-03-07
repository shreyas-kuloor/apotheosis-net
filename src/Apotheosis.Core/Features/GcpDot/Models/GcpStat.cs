namespace Apotheosis.Core.Features.GcpDot.Models;

public sealed class GcpStat
{
    /// <summary>
    /// Gets or sets the time stamp of a GCP stat.
    /// </summary>
    public long Time { get; set; }

    /// <summary>
    /// Gets or sets the value of a GCP stat.
    /// </summary>
    public double Value { get; set; }
}