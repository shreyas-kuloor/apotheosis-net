using Newtonsoft.Json;

namespace Apotheosis.Core.Components.ImageGen.Models;

public sealed class StableDiffusionTextRequest
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