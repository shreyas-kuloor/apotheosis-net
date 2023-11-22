namespace Apotheosis.Components.ImageGen.Models;

public class StableDiffusionResponse
{
    /// <summary>
    /// Gets or sets the list of images returned by Stable Diffusion.
    /// </summary>
    public IEnumerable<string>? Images { get; set; }
}