namespace Apotheosis.Core.Features.MediaRequest.Models.Radarr.Response;

public class Rating
{
    [JsonProperty("votes")]
    public int Votes { get; set; }

    [JsonProperty("value")]
    public double Value { get; set; }

    [JsonProperty("type")]
    public string? Type { get; set; }
}
