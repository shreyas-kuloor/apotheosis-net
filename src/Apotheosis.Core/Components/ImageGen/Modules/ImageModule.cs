using Apotheosis.Core.Components.ImageGen.Interfaces;
using NetCord;
using NetCord.Rest;
using NetCord.Services.ApplicationCommands;

namespace Apotheosis.Core.Components.ImageGen.Modules;

public sealed class ImageModule(IImageGenService imageGenService) : ApplicationCommandModule<SlashCommandContext>
{
    [SlashCommand("image", "Generate an image from the provided prompt and (optional) sampling steps.")]
    public async Task GenerateImageAsync(string prompt, [SlashCommandParameter(Name = "sampling_steps")]int? samplingSteps)
    {
        await RespondAsync(InteractionCallback.DeferredMessage());
        var imageBase64 = await imageGenService.GenerateImageBase64FromPromptAsync(prompt, samplingSteps);

        if (string.IsNullOrWhiteSpace(imageBase64))
        {
            await FollowupAsync(new InteractionMessageProperties
            {
                Content = "There was a problem with the image generation and no image was generated. Please try again later.",
                Flags = MessageFlags.Ephemeral
            });
            return;
        }

        var imageBytes = Convert.FromBase64String(imageBase64);
        var imageStream = new MemoryStream(imageBytes);
        
        await FollowupAsync(new InteractionMessageProperties
        {
            Attachments = new List<AttachmentProperties>
            {
                new("generated_image.png", imageStream)
            }
        });
    }
}