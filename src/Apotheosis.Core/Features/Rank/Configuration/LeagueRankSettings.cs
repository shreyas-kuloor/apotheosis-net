namespace Apotheosis.Core.Features.Rank.Configuration;

public sealed class LeagueRankSettings
{
    public const string Name = "LeagueRank";

    /// <summary>
    /// Gets or sets the League rank Base URL.
    /// </summary>
    public Uri? BaseUrl { get; set; }
}