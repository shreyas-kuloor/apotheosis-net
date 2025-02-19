namespace Apotheosis.Core.Features.Recap.Configuration;

public sealed class LeagueRecapSettings
{
    public const string Name = "LeagueRecap";

    /// <summary>
    /// Gets or sets the League Recap URL.
    /// </summary>
    public Uri? Url { get; set; }
}