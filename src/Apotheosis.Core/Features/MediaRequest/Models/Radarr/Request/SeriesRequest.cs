using Newtonsoft.Json;

namespace Apotheosis.Core.Features.MediaRequest.Models.Radarr.Request;
public sealed class SeriesRequest
{
    [JsonProperty("title")]
    public string? Title { get; set; }

    [JsonProperty("tvdbId")]
    public int TvdbId { get; set; }

    [JsonProperty("rootFolderPath")]
    public string? RootFolderPath { get; set; }

    [JsonProperty("monitored")]
    public bool Monitored { get; set; }

    [JsonProperty("qualityProfileId")]
    public int QualityProfileId { get; set; }

    [JsonProperty("addOptions")]
    public AddOptions? AddOptions { get; set; }
}
