using Newtonsoft.Json;

namespace Apotheosis.Core.Features.MediaRequest.Models.Shared;
public class QualityProfile
{
    [JsonProperty("name")]
    public string Name { get; set; } = default!;

    [JsonProperty("id")]
    public int Id { get; set; }
}
