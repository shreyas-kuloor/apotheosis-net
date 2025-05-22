namespace Apotheosis.Core.Features.MediaRequest.Models.Radarr.Request;
public sealed class MovieRequest
{
    [JsonProperty("title")]
    public string? Title { get; set; }

    [JsonProperty("tmdbId")]
    public int TmdbId { get; set; }

    [JsonProperty("rootFolderPath")]
    public string? RootFolderPath { get; set; }

    [JsonProperty("monitored")]
    public bool Monitored { get; set; }

    [JsonProperty("qualityProfileId")]
    public int QualityProfileId { get; set; }

    [JsonProperty("addOptions")]
    public AddOptions? AddOptions { get; set; }
}
