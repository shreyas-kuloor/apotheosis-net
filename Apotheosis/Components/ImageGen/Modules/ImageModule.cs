using Apotheosis.Components.ImageGen.Interfaces;
using Discord.Interactions;

namespace Apotheosis.Components.ImageGen.Modules;

public sealed class ImageModule : InteractionModuleBase<SocketInteractionContext>
{
    private readonly IImageGenService _imageGenService;

    public ImageModule(IImageGenService imageGenService)
    {
        _imageGenService = imageGenService;
    }
    
    [SlashCommand("image", "Generate an image from the provided prompt and (optional) sampling steps.")]
    public async Task GenerateImageAsync(string prompt, int? samplingSteps)
    {
        await DeferAsync(ephemeral: true);
        var imageBase64 = await _imageGenService.GenerateImageBase64FromPromptAsync(prompt, samplingSteps);

        if (string.IsNullOrWhiteSpace(imageBase64))
        {
            await FollowupAsync("There was a problem with the image generation and no image was generated. Please try again later.", ephemeral: true);
            return;
        }

        var imageBytes = Convert.FromBase64String(imageBase64);
        var imageStream = new MemoryStream(imageBytes);

        await Context.Interaction.FollowupWithFileAsync(imageStream, "generated_image.png");
    }
}