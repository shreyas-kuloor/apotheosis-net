namespace Apotheosis.Components.ImageGen.Configuration;

public class ImageGenSettings
{
    /// <summary>
    /// Gets or sets the stable diffusion base URL.
    /// </summary>
    public Uri? StableDiffusionBaseUrl { get; set; }
    
    /// <summary>
    /// Gets or sets the stable diffusion sampling steps.
    /// </summary>
    public int StableDiffusionSamplingSteps { get; set; }
}