namespace Apotheosis.Core.Features.Rank.Configuration;

public sealed class LeagueStatsSettings
{
    public const string Name = "LeagueStats";

    /// <summary>
    /// Gets or sets the Radarr Base URL.
    /// </summary>
    public Uri? BaseUrl { get; set; }
}