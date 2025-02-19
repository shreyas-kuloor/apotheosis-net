namespace Apotheosis.Core.Features.Rank.Configuration;

public sealed class LeagueRankSettings
{
    public const string Name = "LeagueRank";

    /// <summary>
    /// Gets or sets the League rank URL.
    /// </summary>
    public Uri? Url { get; set; }
}