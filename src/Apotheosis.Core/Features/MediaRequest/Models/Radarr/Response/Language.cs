using Newtonsoft.Json;

namespace Apotheosis.Core.Features.MediaRequest.Models.Radarr.Response;

public class Language
{
    [JsonProperty("id")]
    public int Id { get; set; }

    [JsonProperty("name")]
    public string? Name { get; set; }
}
