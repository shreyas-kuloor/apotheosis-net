using Apotheosis.Core.Features.GcpDot.Interfaces;
using Apotheosis.Core.Utils;
using NetCord.Rest;
using SixLabors.ImageSharp.Formats.Png;

namespace Apotheosis.Core.Features.GcpDot.Modules;

public sealed class GcpDotModule(IGcpDotService gcpDotService) : ApplicationCommandModule<SlashCommandContext>
{
    [SlashCommand("dot", "Get the real-time GCP Dot")]
    public async Task GetGcpDotAsync()
    {
        var (centerColor, edgeColor) = await gcpDotService.GetGcpDotAsync();

        var gcpDotImage = ImageUtils.CreateCircleImage(centerColor, edgeColor, 50);

        using var ms = new MemoryStream();
        await gcpDotImage.SaveAsync(ms, new PngEncoder());
        ms.Seek(0, SeekOrigin.Begin);

        await RespondAsync(InteractionCallback.Message(new InteractionMessageProperties().AddAttachments(new AttachmentProperties("gcp_dot.png", ms))));
    }
}