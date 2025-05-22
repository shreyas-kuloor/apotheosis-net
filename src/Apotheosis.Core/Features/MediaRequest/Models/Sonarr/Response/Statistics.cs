namespace Apotheosis.Core.Features.MediaRequest.Models.Sonarr.Response;

public class Statistics
{
    [JsonProperty("episodeCount")]
    public int EpisodeCount { get; set; }

    [JsonProperty("totalEpisodeCount")]
    public int TotalEpisodeCount { get; set; }

    [JsonProperty("percentOfEpisodes")]
    public double PercentOfEpisodes { get; set; }
}
