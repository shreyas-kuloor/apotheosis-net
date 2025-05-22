namespace Apotheosis.Core.Features.MediaRequest.Models.Radarr.Request;
public sealed class AddOptions
{
    [JsonProperty("searchForMovie")]
    public bool SearchForMovie { get; set; }
}
