namespace Apotheosis.Core.Features.ImageGen.Interfaces;

public interface IImageGenService
{
    Task<string?> GenerateImageBase64FromPromptAsync(string prompt, int? samplingSteps);
}