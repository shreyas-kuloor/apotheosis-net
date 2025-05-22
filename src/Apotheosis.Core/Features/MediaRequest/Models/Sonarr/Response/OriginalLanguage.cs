namespace Apotheosis.Core.Features.MediaRequest.Models.Sonarr.Response;

public class OriginalLanguage
{
    [JsonProperty("id")]
    public int Id { get; set; }

    [JsonProperty("name")]
    public string? Name { get; set; }
}
