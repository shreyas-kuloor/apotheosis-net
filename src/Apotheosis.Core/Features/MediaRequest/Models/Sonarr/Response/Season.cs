namespace Apotheosis.Core.Features.MediaRequest.Models.Sonarr.Response;

public class Season
{
    [JsonProperty("seasonNumber")]
    public int SeasonNumber { get; set; }

    [JsonProperty("statistics")]
    public Statistics? Statistics { get; set; }
}
