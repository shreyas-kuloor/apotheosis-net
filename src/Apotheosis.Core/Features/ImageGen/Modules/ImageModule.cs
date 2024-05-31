using Apotheosis.Core.Features.FeatureFlags.Configuration;
using Apotheosis.Core.Features.ImageGen.Interfaces;
using NetCord.Rest;

namespace Apotheosis.Core.Features.ImageGen.Modules;

public sealed class ImageModule(
    IImageGenService imageGenService,
    IOptions<FeatureFlagSettings> featureFlagOptions) : ApplicationCommandModule<SlashCommandContext>
{
    readonly FeatureFlagSettings featureFlagSettings = featureFlagOptions.Value;

    [SlashCommand("image", "Generate an image from the provided prompt and (optional) sampling steps.")]
    public async Task GenerateImageAsync(string prompt, [SlashCommandParameter(Name = "sampling_steps")] int? samplingSteps)
    {
        if (!featureFlagSettings.ImageGenEnabled)
        {
            await RespondAsync(InteractionCallback.Message(
                new InteractionMessageProperties()
                .WithContent("This command is currently disabled. Please try again later!")
                .WithFlags(MessageFlags.Ephemeral)));
            return;
        }

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