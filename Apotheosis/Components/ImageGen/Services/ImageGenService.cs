using Apotheosis.Components.ImageGen.Configuration;
using Apotheosis.Components.ImageGen.Interfaces;
using Apotheosis.Components.ImageGen.Models;

namespace Apotheosis.Components.ImageGen.Services;

public sealed class ImageGenService(
    IImageGenNetworkDriver imageGenNetworkDriver,
    ImageGenSettings imageGenSettings)
    : IImageGenService
{
    public async Task<string?> GenerateImageBase64FromPromptAsync(string prompt, int? samplingSteps)
    {
        var imageGenRequest = new StableDiffusionTextRequest
        {
            Prompt = prompt,
            Steps = samplingSteps ?? imageGenSettings.StableDiffusionSamplingSteps
        };
        
        var imageGenResponse = await imageGenNetworkDriver.SendRequestAsync<StableDiffusionResponse>("/txt2img", HttpMethod.Post, imageGenRequest);

        return imageGenResponse.Images?.FirstOrDefault();
    }
}