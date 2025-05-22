using Apotheosis.Core.Features.GcpDot.Interfaces;
using Apotheosis.Server.Utils;
using Microsoft.Extensions.Logging;
using SixLabors.ImageSharp.Formats.Png;

namespace Apotheosis.Server.Features.GcpDot.Modules;

public sealed class GcpDotModule(
    IGcpDotService gcpDotService,
    ILogger<GcpDotModule> logger,
    IOptions<FeatureFlagSettings> featureFlagOptions) : ApplicationCommandModule<SlashCommandContext>
{
    readonly FeatureFlagSettings _featureFlagSettings = featureFlagOptions.Value;

    [SlashCommand("dot", "Get the real-time GCP Dot")]
    public async Task GetGcpDotAsync()
    {
        if (!_featureFlagSettings.GcpDotEnabled)
        {
            await RespondAsync(InteractionCallback.Message(
                new InteractionMessageProperties()
                .WithContent("This command is currently disabled. Please try again later!")
                .WithFlags(MessageFlags.Ephemeral)));
            return;
        }

        logger.LogInformation("{User} used /dot", Context.User.GlobalName);

        var (centerColor, edgeColor) = await gcpDotService.GetGcpDotAsync();

        var gcpDotImage = ImageUtils.CreateCircleImage(centerColor, edgeColor, 50);

        using var ms = new MemoryStream();
        await gcpDotImage.SaveAsync(ms, new PngEncoder());
        ms.Seek(0, SeekOrigin.Begin);

        await RespondAsync(InteractionCallback.Message(new InteractionMessageProperties().AddAttachments(new AttachmentProperties("gcp_dot.png", ms))));
    }
}