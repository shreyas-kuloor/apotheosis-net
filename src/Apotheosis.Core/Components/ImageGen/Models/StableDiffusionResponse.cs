namespace Apotheosis.Core.Components.ImageGen.Models;

public sealed class StableDiffusionResponse
{
    /// <summary>
    /// Gets or sets the list of images returned by Stable Diffusion.
    /// </summary>
    public IEnumerable<string>? Images { get; set; }
}