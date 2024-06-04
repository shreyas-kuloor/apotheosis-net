using Newtonsoft.Json;

namespace Apotheosis.Core.Features.MediaRequest.Models.Shared;
public sealed class RootFolder
{
    [JsonProperty("path")]
    public string? Path { get; set; }

    [JsonProperty("accessible")]
    public bool Accessible { get; set; }

    [JsonProperty("freeSpace")]
    public long FreeSpace { get; set; }
}
