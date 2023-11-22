using Newtonsoft.Json;

namespace Apotheosis.Components.ImageGen.Models;

public class StableDiffusionTextRequest
{
    /// <summary>
    /// Gets or sets the text prompt for the image generation request.
    /// </summary>
    [JsonProperty("prompt")]
    public string? Prompt { get; set; }
    
    /// <summary>
    /// Gets or sets the sampling step count for the image generation request.
    /// </summary>
    [JsonProperty("steps")]
    public int Steps { get; set; }
}