using Apotheosis.Components.ImageGen.Configuration;
using Apotheosis.Components.ImageGen.Interfaces;
using Apotheosis.Components.ImageGen.Models;
using Microsoft.Extensions.Options;

namespace Apotheosis.Components.ImageGen.Services;

public sealed class ImageGenService : IImageGenService
{
    private readonly IImageGenNetworkDriver _imageGenNetworkDriver;
    private readonly ImageGenSettings _imageGenSettings;

    public ImageGenService(
        IImageGenNetworkDriver imageGenNetworkDriver,
        IOptions<ImageGenSettings> imageGenOptions)
    {
        _imageGenNetworkDriver = imageGenNetworkDriver;
        _imageGenSettings = imageGenOptions.Value;
    }
    
    public async Task<string?> GenerateImageBase64FromPromptAsync(string prompt, int? samplingSteps)
    {
        var imageGenRequest = new StableDiffusionTextRequest
        {
            Prompt = prompt,
            Steps = samplingSteps ?? _imageGenSettings.StableDiffusionSamplingSteps
        };
        
        var imageGenResponse = await _imageGenNetworkDriver.SendRequestAsync<StableDiffusionResponse>("/txt2img", HttpMethod.Post, imageGenRequest);

        return imageGenResponse.Images?.FirstOrDefault();
    }
}