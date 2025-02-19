namespace Apotheosis.Core.Features.Stats.Configuration;

public sealed class LeagueStatsSettings
{
    public const string Name = "LeagueStats";

    /// <summary>
    /// Gets or sets the League Stats Base URL.
    /// </summary>
    public Uri? BaseUrl { get; set; }
}