namespace Apotheosis.Core.Features.FeatureFlags.Configuration;
public sealed class FeatureFlagSettings
{
    public const string Name = "FeatureFlags";

    public bool ChatEnabled { get; set; }

    public bool ConverseEnabled { get; set; }

    public bool GcpDotEnabled { get; set; }

    public bool RankEnabled { get; set; }

    public bool ImageGenEnabled { get; set; }

    public bool JoinEnabled { get; set; }

    public bool LeaveEnabled { get; set; }

    public bool RequestMovieEnabled { get; set; }

    public bool RequestSeriesEnabled { get; set; }

    public bool SpeakEnabled { get; set; }

    public bool VoicesEnabled { get; set; }
}
