namespace Apotheosis.Core.Features.MediaRequest.Models.Sonarr.Response;

public class Ratings
{
    [JsonProperty("votes")]
    public int Votes { get; set; }

    [JsonProperty("value")]
    public double Value { get; set; }
}