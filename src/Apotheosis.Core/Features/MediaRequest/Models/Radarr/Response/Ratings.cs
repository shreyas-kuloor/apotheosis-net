using Newtonsoft.Json;

namespace Apotheosis.Core.Features.MediaRequest.Models.Radarr.Response;

public class Ratings
{
    [JsonProperty("imdb")]
    public Rating? Imdb { get; set; }

    [JsonProperty("tmdb")]
    public Rating? Tmdb { get; set; }

    [JsonProperty("metacritic")]
    public Rating? Metacritic { get; set; }

    [JsonProperty("rottenTomatoes")]
    public Rating? RottenTomatoes { get; set; }
}