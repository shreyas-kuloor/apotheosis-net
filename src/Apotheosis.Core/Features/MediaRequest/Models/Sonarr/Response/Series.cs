using Newtonsoft.Json;

namespace Apotheosis.Core.Features.MediaRequest.Models.Sonarr.Response;

public class Series
{
    [JsonProperty("id")]
    public int Id { get; set; }

    [JsonProperty("title")]
    public string Title { get; set; } = default!;

    [JsonProperty("status")]
    public string? Status { get; set; }

    [JsonProperty("ended")]
    public bool Ended { get; set; }

    [JsonProperty("overview")]
    public string? Overview { get; set; }

    [JsonProperty("airTime")]
    public string? AirTime { get; set; }

    [JsonProperty("originalLanguage")]
    public OriginalLanguage? OriginalLanguage { get; set; }

    [JsonProperty("remotePoster")]
    public string? RemotePoster { get; set; }

    [JsonProperty("seasons")]
    public List<Season> Seasons { get; set; } = [];

    [JsonProperty("year")]
    public int Year { get; set; }

    [JsonProperty("runtime")]
    public int Runtime { get; set; }

    [JsonProperty("tvdbId")]
    public int TvdbId { get; set; }

    [JsonProperty("imdbId")]
    public string? ImdbId { get; set; }

    [JsonProperty("isAvailable")]
    public bool IsAvailable { get; set; }

    [JsonProperty("hasFile")]
    public bool HasFile { get; set; }

    [JsonProperty("monitored")]
    public bool Monitored { get; set; }

    [JsonProperty("genres")]
    public List<string> Genres { get; set; } = [];

    [JsonProperty("added")]
    public DateTimeOffset Added { get; set; }

    [JsonProperty("ratings")]
    public Ratings? Ratings { get; set; }
}