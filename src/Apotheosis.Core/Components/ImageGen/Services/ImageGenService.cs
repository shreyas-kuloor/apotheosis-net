using Apotheosis.Core.Components.ImageGen.Configuration;
using Apotheosis.Core.Components.ImageGen.Interfaces;
using Apotheosis.Core.Components.ImageGen.Models;

namespace Apotheosis.Core.Components.ImageGen.Services;

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