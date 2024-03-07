using Apotheosis.Core.Features.ImageGen.Models;
using Apotheosis.Core.Features.ImageGen.Configuration;
using Apotheosis.Core.Features.ImageGen.Interfaces;

namespace Apotheosis.Core.Features.ImageGen.Services;

[Scoped]
public sealed class ImageGenService(
    IImageGenNetworkDriver imageGenNetworkDriver,
    IOptions<ImageGenSettings> imageGenOptions)
    : IImageGenService
{
    readonly ImageGenSettings _imageGenSettings = imageGenOptions.Value;

    public async Task<string?> GenerateImageBase64FromPromptAsync(string prompt, int? samplingSteps)
    {
        var imageGenRequest = new StableDiffusionTextRequest
        {
            Prompt = prompt,
            Steps = samplingSteps ?? _imageGenSettings.StableDiffusionSamplingSteps
        };

        var imageGenResponse = await imageGenNetworkDriver.SendRequestAsync<StableDiffusionResponse>("/txt2img", HttpMethod.Post, imageGenRequest);

        return imageGenResponse.Images?.FirstOrDefault();
    }
}