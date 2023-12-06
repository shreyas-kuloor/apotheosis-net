namespace Apotheosis.Core.Components.ImageGen.Interfaces;

public interface IImageGenService
{
    Task<string?> GenerateImageBase64FromPromptAsync(string prompt, int? samplingSteps);
}