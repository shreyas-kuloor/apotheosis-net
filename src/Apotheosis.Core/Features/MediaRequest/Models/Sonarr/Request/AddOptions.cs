using Newtonsoft.Json;

namespace Apotheosis.Core.Features.MediaRequest.Models.Sonarr.Request;
public sealed class AddOptions
{
    [JsonProperty("searchForMissingEpisodes")]
    public bool SearchForMissingEpisodes { get; set; }

    [JsonProperty("searchForCutoffUnmetEpisodes")]
    public bool SearchForCutoffUnmetEpisodes { get; set; }
}
