using Newtonsoft.Json;

namespace Apotheosis.Core.Features.MediaRequest.Models.Radarr.Response;

public class Movie
{
    [JsonProperty("title")]
    public string Title { get; set; } = default!;

    [JsonProperty("originalLanguage")]
    public Language? OriginalLanguage { get; set; }

    [JsonProperty("status")]
    public string? Status { get; set; }

    [JsonProperty("overview")]
    public string? Overview { get; set; }

    [JsonProperty("inCinemas")]
    public DateTimeOffset InCinemas { get; set; }

    [JsonProperty("physicalRelease")]
    public DateTimeOffset PhysicalRelease { get; set; }

    [JsonProperty("digitalRelease")]
    public DateTimeOffset DigitalRelease { get; set; }

    [JsonProperty("remotePoster")]
    public string? RemotePoster { get; set; }

    [JsonProperty("year")]
    public int Year { get; set; }

    [JsonProperty("isAvailable")]
    public bool IsAvailable { get; set; }

    [JsonProperty("hasFile")]
    public bool HasFile { get; set; }

    [JsonProperty("monitored")]
    public bool Monitored { get; set; }

    [JsonProperty("runtime")]
    public int Runtime { get; set; }

    [JsonProperty("imdbId")]
    public string? ImdbId { get; set; }

    [JsonProperty("tmdbId")]
    public int TmdbId { get; set; }

    [JsonProperty("genres")]
    public List<string> Genres { get; set; } = [];

    [JsonProperty("ratings")]
    public Ratings? Ratings { get; set; }

    [JsonProperty("popularity")]
    public double Popularity { get; set; }
}